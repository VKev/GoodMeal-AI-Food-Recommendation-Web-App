using Application.Abstractions.Messaging;
using Domain.Repositories;
using FluentValidation;
using SharedLibrary.Common;
using SharedLibrary.Common.ResponseModel;

namespace Application.Users.Commands;

public sealed record EditUserNameCommand(Guid userId, string name) : ICommand;

public class EditUserNameValidator : AbstractValidator<EditUserNameCommand>
{
    public EditUserNameValidator()
    {
        RuleFor(x => x.userId).NotEmpty();
        RuleFor(x => x.name).NotEmpty();
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