namespace Application.Business.Commands.UpdateBusinessCommand;

public record UpdateBusinessRequest(
    string Name,
    string? Description,
    string? Address,
    string? Phone,
    string? Email,
    string? Website
); 