namespace Application.Business.Commands.CreateBusinessCommand;

public record CreateBusinessRequest(
    string Name,
    string? Description,
    string? Address,
    string? Phone,
    string? Email,
    string? Website,
    string CreateReason
); 