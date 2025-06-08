using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.Contracts.UserCreating
{
    public class GuestCreatedEvent
    {
        public Guid CorrelationId {get; set;}
    }
}