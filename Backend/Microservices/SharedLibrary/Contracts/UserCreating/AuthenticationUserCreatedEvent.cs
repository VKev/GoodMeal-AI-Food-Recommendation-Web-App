namespace SharedLibrary.Contracts.UserCreating
{
    public class AuthenticationUserCreatedEvent
    {
        public Guid CorrelationId {get; set;}
        public string Name {get; set;} = null!;
        public string Email {get; set;} = null!;
        
        public string IdentityID {get; set;} = null!;
    }
}