namespace SharedLibrary.Contracts.UserDeleted
{
    public class UserDeletedEvent
    {
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public string IdentityId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
        public string DeletedBy { get; set; } = "System";
    }
}