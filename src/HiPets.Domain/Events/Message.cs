using System;
using System.Collections.Generic;
using System.Text;

namespace HiPets.Domain.Events
{
    public abstract class Message
    {
        public string Action { get; protected set; }
        public Guid AggregateId { get; protected set; }

        protected Message()
        {
            Action = GetType().Name;
        }
    }
}
