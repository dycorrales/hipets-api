using HiPets.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace HiPets.Domain.Validations
{
    public abstract class Validation<T> : AbstractValidator<T> where T : Entity
    {
        public void ValidateEntity()
        {
            RuleFor(entity => entity.Id)
                .NotEmpty().WithMessage("Identificador inválido");

            RuleFor(entity => entity.Status)
                .IsInEnum().WithMessage("Estatus inválido");
        }

        public ValidationResult ValidationResult { get; protected set; }
    }
}
