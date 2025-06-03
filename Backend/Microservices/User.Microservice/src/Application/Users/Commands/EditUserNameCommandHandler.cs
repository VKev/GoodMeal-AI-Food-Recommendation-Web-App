using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Domain.Repositories;
using FluentValidation;
using SharedLibrary.Common.ResponseModel;

namespace Application.Users.Commands;

public sealed record EditUserNameCommand(string userId, string name) : ICommand;

public class EditUserNameValidator : AbstractValidator<RemoveUserRoleCommand>
{
    public EditUserNameValidator()
    {
        RuleFor(x => x.userId).NotEmpty();
        RuleFor(x => x.roleId).NotEmpty();
    }
}

internal sealed class EditUserNameCommandHandler : ICommandHandler<EditUserNameCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditUserNameCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(EditUserNameCommand request, CancellationToken cancellationToken)
    {
        await _userRepository.EditName(request.userId, request.name, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success("User name updated.");
    }
}