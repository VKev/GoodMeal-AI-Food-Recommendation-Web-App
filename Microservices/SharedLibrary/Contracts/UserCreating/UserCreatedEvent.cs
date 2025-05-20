using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.Contracts.UserCreating
{
    public class UserCreatedEvent
    {
        public Guid CorrelationId {get; set;}
        public string Name {get; set;} = null!;
        public string Email {get; set;} = null!;
    }
}