using System.Security.Claims;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record SoftDeletePromptSessionCommand(
    Guid Id
) : ICommand;

internal sealed class SoftDeletePromptSessionHandler : ICommandHandler<SoftDeletePromptSessionCommand>
{
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SoftDeletePromptSessionHandler(IPromptSessionRepository promptSessionRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _promptSessionRepository = promptSessionRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(SoftDeletePromptSessionCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Result.Failure(new Error("Auth.Unauthoried", "User is not authenticated"));
        }

        var userId = userIdClaim.Value;
        await _promptSessionRepository.SoftDeleteByIdAsync(request.Id, userId, cancellationToken);
        return Result.Success();
    }
}