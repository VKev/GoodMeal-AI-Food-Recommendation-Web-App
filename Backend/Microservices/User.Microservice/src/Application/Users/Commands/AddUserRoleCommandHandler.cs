using SharedLibrary.Common.Messaging;
using AutoMapper;
using Domain.Repositories;
using FluentValidation;
using SharedLibrary.Common.ResponseModel;
using MassTransit;
using SharedLibrary.Contracts.UserRoleChanged;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Common;

namespace Application.Users.Commands;

public sealed record AddUserRoleCommand(Guid userId, Guid roleId) : ICommand;

public class EditUserRoleValidator : AbstractValidator<AddUserRoleCommand>
{
    public EditUserRoleValidator()
    {
        RuleFor(x => x.userId).NotEmpty();
        RuleFor(x => x.roleId).NotEmpty();
    }
}

internal sealed class AddUserRoleCommandHandler : ICommandHandler<AddUserRoleCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AddUserRoleCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
    {
        var userBeforeUpdate = await _userRepository.GetByIdWithRolesAsync(request.userId, cancellationToken);
        if (userBeforeUpdate == null)
        {
            return Result.Failure(new SharedLibrary.Common.ResponseModel.Error("UserNotFound", "User not found"));
        }

        var oldRoles = userBeforeUpdate.UserRoles.Select(ur => ur.Role.RoleName).ToList();

        await _userRepository.AddRole(request.userId, request.roleId, cancellationToken);
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

        return Result.Success();
    }
}