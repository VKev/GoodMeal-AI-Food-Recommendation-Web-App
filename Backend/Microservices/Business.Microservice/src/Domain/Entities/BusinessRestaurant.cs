using System;

namespace Domain.Entities;

public partial class BusinessRestaurant
{
    public Guid Id { get; set; }
    
    public Guid BusinessId { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool? IsDisable { get; set; }
    
    public DateTime? DisableAt { get; set; }
    
    public string? DisableBy { get; set; }
} 