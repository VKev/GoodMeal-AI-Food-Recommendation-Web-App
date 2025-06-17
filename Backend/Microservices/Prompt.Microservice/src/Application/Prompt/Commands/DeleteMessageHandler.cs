using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record DeleteMessageCommand(
    Guid Id
) : ICommand;

internal sealed class DeleteMessageHandler : ICommandHandler<DeleteMessageCommand>
{
    private readonly IMessageRepository _messageRepository;

    public DeleteMessageHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
    }


    public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageRepository.DeleteByIdAsync(request.Id, cancellationToken);
        return Result.Success();
    }
}