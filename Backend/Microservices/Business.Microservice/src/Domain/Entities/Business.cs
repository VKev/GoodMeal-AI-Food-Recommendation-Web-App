using System;

namespace Domain.Entities;

public partial class Business
{
    public Guid Id { get; set; }
    
    public string? OwnerId { get; set; } // User ID from Auth service
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? Address { get; set; }
    
    public string? Phone { get; set; }
    
    public string? Email { get; set; }
    
    public string? Website { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public string? CreatedBy { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool? IsDisable { get; set; }
    
    public DateTime? DisableAt { get; set; }
    
    public string? DisableBy { get; set; }
    
    // Navigation property
    public virtual ICollection<BusinessRestaurant> BusinessRestaurants { get; set; } = new List<BusinessRestaurant>();
} 