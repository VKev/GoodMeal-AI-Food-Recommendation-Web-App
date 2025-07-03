namespace Application.Prompt.Queries;

public record GetPromptSessionResponse(
    Guid Id,
    string UserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    bool? IsDeleted,
    string SessionName
);