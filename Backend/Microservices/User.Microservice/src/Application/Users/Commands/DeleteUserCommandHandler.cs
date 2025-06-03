using SharedLibrary.Common.ResponseModel;
using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Domain.Repositories;
using FluentValidation;

namespace Application.Users.Commands
{
    public sealed record DeleteUserCommand(string UserId) : ICommand;

    public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    internal sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _userRepository.RemoveUserAsync(request.UserId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success("User successfully deleted.");
        }
    }
} 