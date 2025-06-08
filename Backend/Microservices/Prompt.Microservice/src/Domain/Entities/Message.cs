using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Message
{
    public Guid Id { get; set; }

    public Guid PromptSessionId { get; set; }

    public string Sender { get; set; } = null!;

    public string Message1 { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public virtual ICollection<MessageRestaurant> MessageRestaurants { get; set; } = new List<MessageRestaurant>();

    public virtual PromptSession PromptSession { get; set; } = null!;
}
