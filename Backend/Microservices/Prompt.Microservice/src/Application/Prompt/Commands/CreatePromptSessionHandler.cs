using System.Security.Claims;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;
using Application.Prompt.Queries;

namespace Application.Prompt.Commands;

public sealed record CreatePromptSessionCommand(
) : ICommand<GetPromptSessionResponse>;

internal sealed class CreatePromptSessionHandler : ICommandHandler<CreatePromptSessionCommand, GetPromptSessionResponse>
{
    private readonly IPromptSessionRepository _promptSessionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreatePromptSessionHandler(IPromptSessionRepository promptSessionRepository,
        IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _promptSessionRepository = promptSessionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<GetPromptSessionResponse>> Handle(CreatePromptSessionCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Result.Failure<GetPromptSessionResponse>(new Error("Auth.Unauthorized", "User is not authenticated"));
        }

        var userId = userIdClaim.Value;
        var promptSession = new PromptSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            SessionName = "New chat",
            IsDeleted = false
        };
        
        await _promptSessionRepository.AddAsync(promptSession, cancellationToken);
        
        var response = _mapper.Map<GetPromptSessionResponse>(promptSession);
        return Result.Success(response);
    }
}