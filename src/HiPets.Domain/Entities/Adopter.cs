using HiPets.Domain.Helpers.ValueObjects;
using HiPets.Domain.Validations;
using System;
using System.Collections.Generic;
using HiPets.Domain.Helpers;
using HiPets.Domain.Interfaces;

namespace HiPets.Domain.Entities
{
    public sealed class Adopter : Entity, IValidation
    {
        public string Name { get; private set; }
        public ContactInfo ContactInfo { get; private set; }

        public ICollection<Animal> Animals { get; protected set; }

        protected Adopter() { }

        public Adopter(string name, string phoneNumber, string email) : base(Guid.NewGuid(), Status.Active)
        {
            Name = name;
            ContactInfo = new ContactInfo(phoneNumber, email);
        }

        public Adopter(Guid id, string name, string phoneNumber, string email) : base(id, Status.Active)
        {
            Name = name;
            ContactInfo = new ContactInfo(phoneNumber, email);
        }

        public void UpdateAdopter(string name, string phoneNumber, string email)
        {
            Name = name;
            ContactInfo.Update(phoneNumber, email);
        }

        public override bool IsValid()
        {
            var validation = new AdopterValidation();
            validation.ValidateAdopter();
            ValidationResult = validation.Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
