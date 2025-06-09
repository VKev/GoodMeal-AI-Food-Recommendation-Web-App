namespace Application.Prompt.Queries;

public record GetMessageResponse(
    Guid Id,
    Guid PromptSessionId,
    string Sender,
    DateTime CreatedAt,
    bool? IsDeleted,
    DateTime? DeletedAt,
    string? CreateBy,
    DateTime? UpdatedAt,
    string? UpdatedBy,
    string? ResponseMessage,
    string? PromptMessage
);