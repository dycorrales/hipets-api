using HiPets.Domain.Helpers;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Validations;
using System;

namespace HiPets.Domain.Entities
{
    public sealed class Adoption : Entity, IValidation
    {
        public Guid AnimalId { get; private set; }
        public Animal Animal { get; protected set; }
        public Guid AdopterId { get; private set; }
        public Adopter Adopter { get; protected set; }
        public AdoptionStatus AdoptionStatus { get; private set; }
        public string Observation { get; private set; }

        protected Adoption() { }

        public Adoption(Guid animalId, Guid adopterId) : base(Guid.NewGuid(), Status.Active)
        {
            AnimalId = animalId;
            AdopterId = adopterId;
            AdoptionStatus = AdoptionStatus.Requested;
        }

        public void UpdateAdoptionStatus(AdoptionStatus adoptionStatus, string observation = null)
        {
            AdoptionStatus = adoptionStatus;
            Observation = observation;
        }

        public override bool IsValid()
        {
            var validation = new AdoptionValidation();
            validation.ValidateAdoption();
            ValidationResult = validation.Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
