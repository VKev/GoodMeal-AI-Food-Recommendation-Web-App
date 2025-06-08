using Application.Abstractions.Messaging;
using Application.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.Prompt.Queries;

public sealed record GetMessageActiveByIdQuery(Guid PromptSessionId) : IQuery<GetMessageResponse>;

internal sealed class GetMessageActiveByIdQueryHandler : IQueryHandler<GetMessageActiveByIdQuery, GetMessageResponse>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetMessageActiveByIdQueryHandler(IMessageRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }


    public async Task<Result<GetMessageResponse>> Handle(GetMessageActiveByIdQuery request,
        CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetMessageActiveBySessionIdAsync(request.PromptSessionId, cancellationToken);
        var messageResponse = _mapper.Map<GetMessageResponse>(message);
        return Result.Success(messageResponse);
    }
}