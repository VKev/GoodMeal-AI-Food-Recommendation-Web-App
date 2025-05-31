using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Application.Common.ResponseModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Prompt.Queries;

public sealed record GetAllPromptSessionQuery : IQuery<IEnumerable<GetPromptSessionResponse>>;

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
        var sessions = await _promptSessionRepository.GetAllAsync(cancellationToken);
        var response = _mapper.Map<IEnumerable<GetPromptSessionResponse>>(sessions);
        return Result.Success(response);
    }
}