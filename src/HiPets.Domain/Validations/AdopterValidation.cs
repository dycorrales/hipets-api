using FluentValidation;
using HiPets.Domain.Entities;

namespace HiPets.Domain.Validations
{
    public class AdopterValidation : Validation<Adopter>
    {
        public void ValidateAdopter()
        {
            RuleFor(adopter => adopter.Name)
                .MinimumLength(1).WithMessage("O nome deve ter de 1 até 250 caracteres")
                .MaximumLength(150).WithMessage("O nome deve ter de 1 até 250 caracteres")
                .NotEmpty().WithMessage("Nome requerido");

            RuleFor(adopter => adopter.ContactInfo.Email)
                .MinimumLength(1).WithMessage("O email deve ter de 1 até 150 caracteres")
                .MaximumLength(150).WithMessage("O email deve ter de 1 até 150 caracteres")
                .EmailAddress().WithMessage("Email inválido")
                .NotEmpty().WithMessage("Email requerido");

            RuleFor(adopter => adopter.ContactInfo.PhoneNumber)
                .MinimumLength(1).WithMessage("O telefone deve ter de 1 até 11 caracteres")
                .MaximumLength(11).WithMessage("O telefone deve ter de 1 até 11 caracteres")
                .NotEmpty().WithMessage("Telefone requerido");

            ValidateEntity();
        }
    }
}
