using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using Infrastructure.Repositories;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record SoftDeleteMessageCommand(
    Guid Id
) : ICommand;

internal sealed class SoftDeleteMessageHandler : ICommandHandler<SoftDeleteMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteMessageHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SoftDeleteMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageRepository.SoftDeleteByIdAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}