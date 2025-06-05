namespace SharedLibrary.Contracts.UserCreating
{
    public class GuestCreatedFailureEvent
    {
        public Guid CorrelationId {get; set;}

        public string Reason {get; set;}
    }
}