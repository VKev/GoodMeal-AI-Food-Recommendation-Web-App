using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using AutoMapper;
using Domain.Repositories;
using FluentValidation;
using SharedLibrary.Common.ResponseModel;

namespace Application.Users.Commands;

public sealed record AddUserRoleCommand(string userId, string roleId) : ICommand;

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

    public AddUserRoleCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result> Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
    {
        await _userRepository.AddRole(request.userId, request.roleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}