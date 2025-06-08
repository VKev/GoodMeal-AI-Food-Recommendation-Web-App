namespace Application.Prompt.Queries;

public record GetPromptSessionResponse(
    Guid Id,
    Guid UserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);