using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace Application.Sagas
{
    public class UserCreatingSagaData : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public bool UserCreated { get; set; }

        public bool GuestCreated { get; set; }

        public int RetryCount { get; set; }
        public int Version { get; set; }
    }
}