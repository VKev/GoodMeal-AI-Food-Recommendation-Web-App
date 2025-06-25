using System.Security.Claims;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record CreatePromptSessionCommand(
) : ICommand;

internal sealed class CreatePromptSessionHandler : ICommandHandler<CreatePromptSessionCommand>
{
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreatePromptSessionHandler(IPromptSessionRepository promptSessionRepository,
        IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _promptSessionRepository = promptSessionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(CreatePromptSessionCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Result.Failure(new Error("Auth.Unauthoried", "User is not authenticated"));
        }

        var userId = userIdClaim.Value;
        var promptSession = _mapper.Map<PromptSession>(request);
        promptSession.CreatedAt = DateTime.UtcNow;
        promptSession.CreatedBy = userId;
        promptSession.UserId = userId;
        await _promptSessionRepository.AddAsync(promptSession, cancellationToken);
        return Result.Success();
    }
}