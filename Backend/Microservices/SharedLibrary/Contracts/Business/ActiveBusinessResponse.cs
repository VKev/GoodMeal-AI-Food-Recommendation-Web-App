namespace SharedLibrary.Contracts.Business;

public record ActiveBusinessResponse
{
    public string RequestId { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public BusinessDto? Business { get; init; }
} 