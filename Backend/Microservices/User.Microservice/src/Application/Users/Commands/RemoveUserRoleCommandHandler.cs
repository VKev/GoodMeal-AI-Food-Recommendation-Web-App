using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Domain.Repositories;
using FluentValidation;
using SharedLibrary.Common.ResponseModel;
using MassTransit;
using SharedLibrary.Contracts.UserRoleChanged;

namespace Application.Users.Commands;

public sealed record RemoveUserRoleCommand(Guid userId, Guid roleId) : ICommand;

public class RemoveUserRoleValidator : AbstractValidator<RemoveUserRoleCommand>
{
    public RemoveUserRoleValidator()
    {
        RuleFor(x => x.userId).NotEmpty();
        RuleFor(x => x.roleId).NotEmpty();
    }
}

internal sealed class RemoveUserRoleCommandHandler : ICommandHandler<RemoveUserRoleCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public RemoveUserRoleCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        var userBeforeUpdate = await _userRepository.GetByIdWithRolesAsync(request.userId, cancellationToken);
        if (userBeforeUpdate == null)
        {
            return Result.Failure(new SharedLibrary.Common.ResponseModel.Error("UserNotFound", "User not found"));
        }

        var oldRoles = userBeforeUpdate.UserRoles.Select(ur => ur.Role.RoleName).ToList();

        await _userRepository.RemoveRole(request.userId, request.roleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userAfterUpdate = await _userRepository.GetByIdWithRolesAsync(request.userId, cancellationToken);
        var newRoles = userAfterUpdate?.UserRoles.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>();

        if (!string.IsNullOrEmpty(userBeforeUpdate.IdentityId))
        {
            await _publishEndpoint.Publish(new UserRoleChangedEvent
            {
                CorrelationId = Guid.NewGuid(),
                UserId = request.userId,
                IdentityId = userBeforeUpdate.IdentityId,
                Email = userBeforeUpdate.Email,
                Name = userBeforeUpdate.Name,
                NewRoles = newRoles,
                OldRoles = oldRoles,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = "System"
            }, cancellationToken);
        }

        return Result.Success("Role removed from user.");
    }
}