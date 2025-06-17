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

    public SoftDeleteMessageHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Result> Handle(SoftDeleteMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageRepository.SoftDeleteByIdAsync(request.Id, cancellationToken);
        return Result.Success();
    }
}