using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using SharedLibrary.Common;

namespace Application.Prompt.Queries;

public sealed record GetAllPromptSessionQuery()
    : IQuery<IEnumerable<GetPromptSessionResponse>>;

public class
    GetAllPromptSessionQueryHandler : IQueryHandler<GetAllPromptSessionQuery, IEnumerable<GetPromptSessionResponse>>
{
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllPromptSessionQueryHandler(IPromptSessionRepository promptSessionRepository, IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _promptSessionRepository = promptSessionRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<IEnumerable<GetPromptSessionResponse>>> Handle(GetAllPromptSessionQuery request,
        CancellationToken cancellationToken)
    {
        var promptSessions = await _promptSessionRepository.GetAllAsync(cancellationToken);
        var promptSessionResponses = _mapper.Map<IEnumerable<GetPromptSessionResponse>>(promptSessions);
        return Result.Success(promptSessionResponses);
    }
}