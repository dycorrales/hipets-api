using FluentValidation.Results;

namespace HiPets.Domain.Interfaces
{
    public interface IValidation
    {
        bool IsValid();
        ValidationResult ValidationResult { get; set; }
    }
}
