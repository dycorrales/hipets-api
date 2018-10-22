using HiPets.Domain.Helpers;
using System;
using FluentValidation.Results;

namespace HiPets.Domain.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Status Status { get; private set; }
        
        protected Entity() { }

        protected Entity(Guid id, Status status)
        {
            Id = id;
            CreatedAt = DateTime.Today;
            Status = status;
        }

        public void Delete()
        {
            Status = Status.Deleted;
        }

        public abstract bool IsValid();        
        public ValidationResult ValidationResult { get; set; }
    }
}