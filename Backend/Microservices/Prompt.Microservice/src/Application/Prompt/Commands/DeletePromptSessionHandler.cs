using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Application.Common.ResponseModel;
using Domain.Repositories;

namespace Application.Prompt.Commands;

public sealed record DeletePromptSessionCommand(
    Guid Id
) : ICommand;

public class DeletePromptSessionHandler : ICommandHandler<DeletePromptSessionCommand>
{
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePromptSessionHandler(IPromptSessionRepository promptSessionRepository, IUnitOfWork unitOfWork)
    {
        _promptSessionRepository = promptSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeletePromptSessionCommand request, CancellationToken cancellationToken)
    {
        var promptSession = await _promptSessionRepository.GetByIdAsync(request.Id, cancellationToken);
        _promptSessionRepository.Delete(promptSession);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}