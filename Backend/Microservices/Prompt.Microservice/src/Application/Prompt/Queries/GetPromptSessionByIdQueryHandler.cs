using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;
using SharedLibrary.Common;

namespace Application.Prompt.Queries;

public sealed record GetPromptSessionByIdQuery(Guid Id) : IQuery<GetPromptSessionResponse>;

internal sealed class GetPromptSessionByIdQueryHandler : IQueryHandler<GetPromptSessionByIdQuery, GetPromptSessionResponse>
{
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IMapper _mapper;

    public GetPromptSessionByIdQueryHandler(IPromptSessionRepository promptSessionRepository, IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _promptSessionRepository = promptSessionRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<GetPromptSessionResponse>> Handle(GetPromptSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var promptSession = await _promptSessionRepository.GetByIdAsync(request.Id, cancellationToken);
        var promptSessionResponse = _mapper.Map<GetPromptSessionResponse>(promptSession);
        return Result.Success(promptSessionResponse);
    }
}