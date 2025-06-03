using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Domain.Repositories;
using FluentValidation;
using SharedLibrary.Common.ResponseModel;

namespace Application.Users.Commands;

public sealed record RemoveUserRoleCommand(string userId, string roleId) : ICommand;

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

    public RemoveUserRoleCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        await _userRepository.RemoveRole(request.userId, request.roleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success("Role removed from user.");
    }
}