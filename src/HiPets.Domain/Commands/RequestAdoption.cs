using System;
using System.Collections.Generic;
using System.Text;

namespace HiPets.Domain.Commands
{
    public sealed class RequestAdoption : Command
    {
        public Guid AnimalId { get; set; }
        public Guid AdopterId { get; set; }
    }
}
