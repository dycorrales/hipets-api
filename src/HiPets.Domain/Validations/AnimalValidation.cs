using FluentValidation;
using HiPets.Domain.Entities;

namespace HiPets.Domain.Validations
{
    public class AnimalValidation : Validation<Animal>
    {
        public void ValidateAnimal()
        {
            RuleFor(animal => animal.Name)
                .MinimumLength(1).WithMessage("O nome deve ter de 1 até 250 caracteres")
                .MaximumLength(150).WithMessage("O nome deve ter de 1 até 250 caracteres")
                .NotEmpty().WithMessage("Nome requerido");
            
            RuleFor(animal => animal.Breed)
                .MinimumLength(1).WithMessage("A raça deve ter de 1 até 50 caracteres")
                .MaximumLength(50).WithMessage("A raça deve ter de 1 até 50 caracteres")
                .NotEmpty().WithMessage("Raça requerida");

            RuleFor(animal => animal.Age)
                .NotEmpty().WithMessage("Idade requerida");

            RuleFor(animal => animal.PrevalentColor)
                .IsInEnum().WithMessage("Color predominante inválido");

            RuleFor(animal => animal.Behavior)
                .MinimumLength(1).WithMessage("O comportamento deve ter de 1 até 250 caracteres")
                .MaximumLength(250).WithMessage("O comportamento deve ter de 1 até 250 caracteres")
                .NotEmpty().WithMessage("Comportamento requerido");

            RuleFor(animal => animal.AnimalStatus)
                .IsInEnum().WithMessage("Status do animal inválido");

            RuleFor(animal => animal.AnimalType)
                .IsInEnum().WithMessage("Tipo de animal inválido");

            ValidateEntity();
        }
    }
}
