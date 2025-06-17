using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.Prompt.Queries;

public sealed record GetMessageByIdQuery(Guid Id) : IQuery<GetMessageResponse>;

internal sealed class GetMessageByIdQueryHandler : IQueryHandler<GetMessageByIdQuery, GetMessageResponse>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetMessageByIdQueryHandler(IMessageRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetMessageResponse>> Handle(GetMessageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.Id, cancellationToken);
        var messageResponse = _mapper.Map<GetMessageResponse>(message);
        return Result.Success(messageResponse);
    }
}