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

    /// <summary>
    /// Google Map Links
    /// </summary>
    public string? PlaceLink { get; set; }

    /// <summary>
    /// Link website
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// types of restaurant
    /// </summary>
    public string? Types { get; set; }

    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public string? TimeZone { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();

    public virtual ICollection<RestaurantRating> RestaurantRatings { get; set; } = new List<RestaurantRating>();
}
