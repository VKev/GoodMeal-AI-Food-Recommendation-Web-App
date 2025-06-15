using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record CreatePromptSessionCommand(
    Guid Id,
    Guid UserId,
    string CreatedBy
) : ICommand;

internal sealed class CreatePromptSessionHandler : ICommandHandler<CreatePromptSessionCommand>
{
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePromptSessionHandler(IPromptSessionRepository promptSessionRepository, IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _promptSessionRepository = promptSessionRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreatePromptSessionCommand request, CancellationToken cancellationToken)
    {
        var promptSession = _mapper.Map<PromptSession>(request);
        await _promptSessionRepository.AddAsync(promptSession, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}