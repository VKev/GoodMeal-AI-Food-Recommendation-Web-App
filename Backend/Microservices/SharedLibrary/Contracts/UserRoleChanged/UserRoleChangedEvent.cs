namespace SharedLibrary.Contracts.UserRoleChanged
{
    public class UserRoleChangedEvent
    {
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public string IdentityId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public IEnumerable<string> NewRoles { get; set; } = new List<string>();
        public IEnumerable<string> OldRoles { get; set; } = new List<string>();
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string ChangedBy { get; set; } = "System";
    }
} 