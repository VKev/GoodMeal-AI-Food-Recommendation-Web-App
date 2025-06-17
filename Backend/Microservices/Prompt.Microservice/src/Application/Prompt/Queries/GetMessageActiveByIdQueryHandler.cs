using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.Prompt.Queries;

public sealed record GetMessageActiveByIdQuery(Guid PromptSessionId) : IQuery<IEnumerable<GetMessageResponse>>;

internal sealed class GetMessageActiveByIdQueryHandler : IQueryHandler<GetMessageActiveByIdQuery, IEnumerable<GetMessageResponse>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetMessageActiveByIdQueryHandler(IMessageRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }


    public async Task<Result<IEnumerable<GetMessageResponse>>> Handle(GetMessageActiveByIdQuery request,
        CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetMessageActiveBySessionIdAsync(request.PromptSessionId, cancellationToken);
        var messageResponse = _mapper.Map<IEnumerable<GetMessageResponse>>(message);
        return Result.Success(messageResponse);
    }
}