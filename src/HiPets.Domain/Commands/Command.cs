using HiPets.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiPets.Domain.Commands
{
    public abstract class Command : Message, IRequest<string>
    {
        public DateTime Timestamp { get; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }
    }
}
