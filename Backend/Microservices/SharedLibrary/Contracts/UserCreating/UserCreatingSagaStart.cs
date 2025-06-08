namespace SharedLibrary.Contracts.UserCreating
{
    public class UserCreatingSagaStart
    {
        public Guid CorrelationId {get; set;}
        public string Name {get; set;} = null!;
        public string Email {get; set;} = null!;
    }
}