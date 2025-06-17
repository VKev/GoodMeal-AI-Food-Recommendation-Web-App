using Application.Common.GeminiApi;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using Infrastructure;
using Newtonsoft.Json;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record CreateMessageCommand(
    Guid? PromptSessionId,
    string Sender,
    Guid UserId,
    string PromptMessage,
    string ResponseMessage
) : ICommand;

internal sealed class CreateMessageHandler : ICommandHandler<CreateMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IPromptSessionRepository _promptSessionRepository;

    public CreateMessageHandler(
        IMessageRepository messageRepository,
        IPromptSessionRepository promptSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _promptSessionRepository = promptSessionRepository;
    }


    public async Task<Result> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        Guid sessionId = Guid.NewGuid();

        if (request.PromptSessionId == null || request.PromptSessionId == Guid.Empty)
        {
            var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(request.ResponseMessage);
            if (geminiResponse == null)
                return Result.Failure(Error.NullValue);

            var newSession = new PromptSession
            {
                Id = sessionId,
                UserId = request.UserId,
                SessionName = geminiResponse.Title,
                CreatedAt = DateTime.UtcNow
            };

            await _promptSessionRepository.AddAsync(newSession, cancellationToken);
        }
        else
        {
            sessionId = request.PromptSessionId.Value;
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