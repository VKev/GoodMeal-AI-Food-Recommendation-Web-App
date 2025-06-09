using Application.Abstractions.Messaging;
using Application.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.Prompt.Queries;

public sealed record GetAllMessageQuery()
    : IQuery<IEnumerable<GetMessageResponse>>;

internal sealed class GetAllMessageQueryHandler : IQueryHandler<GetAllMessageQuery, IEnumerable<GetMessageResponse>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetAllMessageQueryHandler(IMessageRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetMessageResponse>>> Handle(GetAllMessageQuery request,
        CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetAllAsync(cancellationToken);
        var messageResponses = _mapper.Map<IEnumerable<GetMessageResponse>>(messages);
        return Result.Success(messageResponses);
    }
}