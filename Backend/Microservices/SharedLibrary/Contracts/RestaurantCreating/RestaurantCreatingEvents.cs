namespace SharedLibrary.Contracts.RestaurantCreating;

// Saga start event
public class RestaurantCreatingSagaStart
{
    public Guid CorrelationId { get; set; }
    public Guid BusinessId { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

// Event to trigger restaurant creation in Restaurant microservice
public class CreateRestaurantEvent
{
    public Guid CorrelationId { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

// Success event from Restaurant microservice
public class RestaurantCreatedEvent
{
    public Guid CorrelationId { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
}

// Failure event from Restaurant microservice
public class RestaurantCreatedFailureEvent
{
    public Guid CorrelationId { get; set; }
    public Guid RestaurantId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

// Event to create BusinessRestaurant relationship
public class CreateBusinessRestaurantEvent
{
    public Guid CorrelationId { get; set; }
    public Guid BusinessId { get; set; }
    public Guid RestaurantId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

// Success event for BusinessRestaurant creation
public class BusinessRestaurantCreatedEvent
{
    public Guid CorrelationId { get; set; }
    public Guid BusinessId { get; set; }
    public Guid RestaurantId { get; set; }
}

// Failure event for BusinessRestaurant creation
public class BusinessRestaurantCreatedFailureEvent
{
    public Guid CorrelationId { get; set; }
    public Guid BusinessId { get; set; }
    public Guid RestaurantId { get; set; }
    public string Reason { get; set; } = string.Empty;
} 