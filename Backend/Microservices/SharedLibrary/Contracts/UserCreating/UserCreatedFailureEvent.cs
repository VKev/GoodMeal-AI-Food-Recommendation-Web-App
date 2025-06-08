namespace SharedLibrary.Contracts.UserCreating
{
    public class UserCreatedFailureEvent
    {
        public Guid CorrelationId {get; set;}

        public string Reason {get; set;}
    }
}