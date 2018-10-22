using FluentValidation;
using HiPets.Domain.Entities;

namespace HiPets.Domain.Validations
{
    public class AdoptionValidation : Validation<Adoption>
    {
        public void ValidateAdoption()
        {
            RuleFor(adoption => adoption.AnimalId)
                .NotEmpty().WithMessage("Animal requerido");

            RuleFor(adoption => adoption.AdopterId)
                .NotEmpty().WithMessage("Cliente requerido");

            RuleFor(adoption => adoption.AdoptionStatus)
                .IsInEnum().WithMessage("Status de adoção inválido");

            ValidateEntity();
        }
    }
}
