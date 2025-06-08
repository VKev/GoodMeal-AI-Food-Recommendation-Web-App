using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Food
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDisable { get; set; }

    public DateTime? DisableAt { get; set; }

    public Guid Id { get; set; }

    public Guid? RestaurantId { get; set; }

    public virtual Restaurant? Restaurant { get; set; }
}
