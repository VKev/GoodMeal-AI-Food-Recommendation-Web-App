namespace SharedLibrary.Contracts.Business;

public record ActiveBusinessRequest
{
    public string RequestId { get; init; } = Guid.NewGuid().ToString();
    public Guid BusinessId { get; init; }
    public string UserId { get; init; } = string.Empty;
} 