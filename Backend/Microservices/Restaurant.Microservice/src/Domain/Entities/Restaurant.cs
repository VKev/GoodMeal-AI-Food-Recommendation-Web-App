using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Restaurant
{
    public string? CreatedBy { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDisable { get; set; }

    public DateTime? DisableAt { get; set; }

    public Guid Id { get; set; }

    public string? Phone { get; set; }

    public string? DisableBy { get; set; }

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();

    public virtual ICollection<RestaurantRating> RestaurantRatings { get; set; } = new List<RestaurantRating>();
}
