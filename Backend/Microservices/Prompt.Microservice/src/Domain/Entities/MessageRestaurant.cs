using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Infrastructure;

public partial class MessageRestaurant
{
    public Guid MessageId { get; set; }

    public Guid RestaurantId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public virtual Message Message { get; set; } = null!;
}
