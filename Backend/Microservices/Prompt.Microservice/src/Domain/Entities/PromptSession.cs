using System;
using System.Collections.Generic;

namespace Infrastructure;

public partial class PromptSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public string? SessionName { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
