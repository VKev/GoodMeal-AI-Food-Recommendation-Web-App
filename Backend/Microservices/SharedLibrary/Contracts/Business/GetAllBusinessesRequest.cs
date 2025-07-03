namespace SharedLibrary.Contracts.Business;

public record GetAllBusinessesRequest
{
    public string RequestId { get; init; } = Guid.NewGuid().ToString();
} 