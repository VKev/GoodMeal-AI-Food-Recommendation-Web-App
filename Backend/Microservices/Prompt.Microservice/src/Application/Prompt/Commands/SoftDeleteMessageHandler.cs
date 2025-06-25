using System.Security.Claims;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record SoftDeleteMessageCommand(
    Guid Id
) : ICommand;

internal sealed class SoftDeleteMessageHandler : ICommandHandler<SoftDeleteMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SoftDeleteMessageHandler(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor)
    {
        _messageRepository = messageRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(SoftDeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Result.Failure(new Error("Auth.Unauthoried", "User is not authenticated"));
        }

        var userId = userIdClaim.Value;
        await _messageRepository.SoftDeleteByIdAsync(request.Id, userId, cancellationToken);
        return Result.Success();
    }
}