using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using Domain.Repositories;
using FluentValidation;
using MassTransit;
using SharedLibrary.Common;
using SharedLibrary.Contracts.UserDeleted;

namespace Application.Users.Commands
{
    public sealed record DeleteUserCommand(Guid UserId) : ICommand;

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
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var userBeforeDeletion = await _userRepository.GetByIdWithRolesAsync(request.UserId, cancellationToken);
            if (userBeforeDeletion == null)
            {
                return Result.Failure(new SharedLibrary.Common.ResponseModel.Error("UserNotFound", "User not found"));
            }

            await _userRepository.RemoveUserAsync(request.UserId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrEmpty(userBeforeDeletion.IdentityId))
            {
                await _publishEndpoint.Publish(new UserDeletedEvent
                {
                    CorrelationId = Guid.NewGuid(),
                    UserId = request.UserId,
                    IdentityId = userBeforeDeletion.IdentityId,
                    Email = userBeforeDeletion.Email,
                    Name = userBeforeDeletion.Name,
                    DeletedAt = DateTime.UtcNow,
                    DeletedBy = "System"
                }, cancellationToken);
            }

            return Result.Success("User successfully deleted.");
        }
    }
}