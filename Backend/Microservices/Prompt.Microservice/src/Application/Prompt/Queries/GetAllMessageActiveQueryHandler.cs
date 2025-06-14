using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.Prompt.Queries;

public sealed record GetAllMessageActiveQuery : IQuery<IEnumerable<GetMessageResponse>>;

internal sealed class
    GetAllMessageActiveQueryHandler : IQueryHandler<GetAllMessageActiveQuery, IEnumerable<GetMessageResponse>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetAllMessageActiveQueryHandler(IMessageRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetMessageResponse>>> Handle(GetAllMessageActiveQuery request,
        CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetAllActiveAsync(cancellationToken);
        var messageResponses = _mapper.Map<IEnumerable<GetMessageResponse>>(messages);
        return Result.Success(messageResponses);
    }
}