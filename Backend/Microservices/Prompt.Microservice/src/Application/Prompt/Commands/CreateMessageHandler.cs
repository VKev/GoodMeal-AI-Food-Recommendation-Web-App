using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Application.Common.ResponseModel;
using Domain.Entities;
using Domain.Repositories;
using Newtonsoft.Json;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record CreateMessageCommand(
    Guid PromptSessionId,
    string Sender,
    string PromptMessage,
    string ResponseMessage
) : ICommand;

internal sealed class CreateMessageHandler : ICommandHandler<CreateMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMessageHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            PromptSessionId = request.PromptSessionId,
            Sender = request.Sender,
            PromptMessage = request.PromptMessage,
            ResponseMessage = JsonConvert.SerializeObject(request.ResponseMessage),
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}