using Infrastructure;

namespace Domain.Entities;

public partial class Message
{
    public Guid Id { get; set; }

    public Guid PromptSessionId { get; set; }

    public string Sender { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public string? ResponseMessage { get; set; }

    public string? PromptMessage { get; set; }

    public virtual ICollection<MessageRestaurant> MessageRestaurants { get; set; } = new List<MessageRestaurant>();

    public virtual PromptSession PromptSession { get; set; } = null!;
}
