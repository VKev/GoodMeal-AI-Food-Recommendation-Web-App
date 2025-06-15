using MassTransit;

namespace Application.Sagas
{
    public class AuthenticationUserCreatingSagaData : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public bool AuthenticationUserCreated { get; set; }

        public bool UserCreated { get; set; }

        public int RetryCount { get; set; }
        public int Version { get; set; }
    }
}