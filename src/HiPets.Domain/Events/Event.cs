using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiPets.Domain.Events
{
    public abstract class Event : Message, INotification
    {
        public DateTime CreationDate { get; protected set; }

        protected Event()
        {
            CreationDate = DateTime.Now;
        }
    }
}
