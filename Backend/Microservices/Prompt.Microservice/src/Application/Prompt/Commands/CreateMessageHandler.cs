using System.Security.Claims;
using Application.Common.GeminiApi;
using Domain.Entities;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record CreateMessageCommand(
    Guid? PromptSessionId,
    string Sender,
    string PromptMessage,
    string ResponseMessage
) : ICommand;

internal sealed class CreateMessageHandler : ICommandHandler<CreateMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateMessageHandler(
        IMessageRepository messageRepository,
        IPromptSessionRepository promptSessionRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _messageRepository = messageRepository;
        _promptSessionRepository = promptSessionRepository;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<Result> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        Guid sessionId = Guid.NewGuid();
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Result.Failure(new Error("Auth.Unauthoried", "User is not authenticated"));
        }

        var userId = userIdClaim.Value;
        var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(request.ResponseMessage);
        if (geminiResponse == null)
            return Result.Failure(Error.NullValue);

        if (request.PromptSessionId == null || request.PromptSessionId == Guid.Empty)
        {
            var newSession = new PromptSession
            {
                Id = sessionId,
                UserId = userId,
                SessionName = geminiResponse.Title,
                CreatedAt = DateTime.UtcNow
            };

            await _promptSessionRepository.AddAsync(newSession, cancellationToken);
        }
        else
        {
            sessionId = request.PromptSessionId.Value;
            var existingSession = await _promptSessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (existingSession.SessionName is null)
            {
                existingSession.SessionName = geminiResponse.Title;
                _promptSessionRepository.UpdateFields(existingSession, x => x.SessionName!);
            }
        }

        var message = new Message
        {
            PromptSessionId = sessionId,
            Sender = request.Sender,
            PromptMessage = request.PromptMessage,
            ResponseMessage = JsonConvert.SerializeObject(request.ResponseMessage),
            CreatedAt = DateTime.UtcNow,
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        return Result.Success();
    }
}