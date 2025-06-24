namespace Domain.Entities;

public partial class UserSubscription
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;

    public Guid SubscriptionId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDisable { get; set; }

    public DateTime? DisableAt { get; set; }

    public string? DisableBy { get; set; }

    public virtual Subscription Subscription { get; set; } = null!;
} 