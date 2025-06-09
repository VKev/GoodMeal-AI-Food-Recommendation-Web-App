using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Application.Common.ResponseModel;
using Domain.Repositories;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record DeleteMessageCommand(
    Guid Id
) : ICommand;

internal sealed class DeleteMessageHandler : ICommandHandler<DeleteMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMessageHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageRepository.DeleteByIdAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}