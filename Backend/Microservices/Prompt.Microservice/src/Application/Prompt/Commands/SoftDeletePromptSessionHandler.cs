using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record SoftDeletePromptSessionCommand(
    Guid Id
) : ICommand;

internal sealed class SoftDeletePromptSessionHandler : ICommandHandler<SoftDeletePromptSessionCommand>
{
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeletePromptSessionHandler(IPromptSessionRepository promptSessionRepository, IUnitOfWork unitOfWork)
    {
        _promptSessionRepository = promptSessionRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result> Handle(SoftDeletePromptSessionCommand request, CancellationToken cancellationToken)
    {
        await _promptSessionRepository.SoftDeleteByIdAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}