using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class RestaurantRating
{
    public float? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid Id { get; set; }

    public Guid? RestaurantId { get; set; }

    public Guid? UserId { get; set; }

    public bool? IsDisable { get; set; }

    public DateTime? DisableAt { get; set; }

    public string? DisableBy { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Restaurant? Restaurant { get; set; }
}
