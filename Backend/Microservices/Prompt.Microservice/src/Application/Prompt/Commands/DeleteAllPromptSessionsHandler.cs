using System.Security.Claims;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record DeleteAllPromptSessionsCommand() : ICommand;

internal sealed class DeleteAllPromptSessionsHandler : ICommandHandler<DeleteAllPromptSessionsCommand>
{
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteAllPromptSessionsHandler(IPromptSessionRepository promptSessionRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _promptSessionRepository = promptSessionRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(DeleteAllPromptSessionsCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated"));
        }

        var userId = userIdClaim.Value;
        await _promptSessionRepository.SoftDeleteAllByUserIdAsync(userId, cancellationToken);
        return Result.Success();
    }
}