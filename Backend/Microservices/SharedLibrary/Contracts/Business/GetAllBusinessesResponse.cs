namespace SharedLibrary.Contracts.Business;

public record GetAllBusinessesResponse
{
    public string RequestId { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public IEnumerable<BusinessDto> Businesses { get; init; } = new List<BusinessDto>();
}

public class BusinessDto
{
    public Guid Id { get; set; }
    public string? OwnerId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool? IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool? IsDisable { get; set; }
    public DateTime? DisableAt { get; set; }
    public string? DisableBy { get; set; }
    public string? CreateReason { get; set; }
    public DateTime? ActivatedAt { get; set; }
}