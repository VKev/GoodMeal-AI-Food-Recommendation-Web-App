namespace SharedLibrary.Contracts.UserCreating
{
    public class UserCreatedEvent
    {
        public Guid CorrelationId {get; set;}
        public string Name {get; set;} = null!;
        public string Email {get; set;} = null!;
        public string IdentityId {get; set;} = null!;
        public Guid UserId {get; set;}
    }
}