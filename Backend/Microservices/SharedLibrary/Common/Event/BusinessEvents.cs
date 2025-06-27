namespace SharedLibrary.Common.Event;

public record BusinessActivatedEvent
{
    public Guid BusinessId { get; init; }
    public string OwnerId { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public DateTime ActivatedAt { get; init; }
    public string ActivatedBy { get; init; } = string.Empty;
}

public record BusinessDeactivatedEvent
{
    public Guid BusinessId { get; init; }
    public string OwnerId { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public DateTime DeactivatedAt { get; init; }
    public string DeactivatedBy { get; init; } = string.Empty;
}

public record BusinessEnabledEvent
{
    public Guid BusinessId { get; init; }
    public string OwnerId { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public DateTime EnabledAt { get; init; }
    public string EnabledBy { get; init; } = string.Empty;
}

public record BusinessDisabledEvent
{
    public Guid BusinessId { get; init; }
    public string OwnerId { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public DateTime DisabledAt { get; init; }
    public string DisabledBy { get; init; } = string.Empty;
} 