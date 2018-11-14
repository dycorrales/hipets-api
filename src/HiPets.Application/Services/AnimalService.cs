using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using HiPets.Domain.Interfaces;

namespace HiPets.Application.Services
{
    public sealed class AnimalService : Service<Animal>, IAnimalService
    {
        private readonly IAnimalRepository _repository;

        public AnimalService(IMediatorBus mediator, IUnitOfWork unitOfWork, IAnimalRepository repository) : base(mediator, unitOfWork, repository)
        {
            _repository = repository;
        }

        public PagingResult<Animal> GetAnimals(int page, int pageSize, AnimalStatus animalStatus, string searchString = null, bool? isActive = null)
        {
            Status? status = null;

            if (isActive != null)
            {
                status = isActive.Value ? Status.Active : Status.Inactive;
            }

            return _repository.GetAnimals(page, pageSize, animalStatus, searchString, status);
        }

        public void InsertAnimal(string name, string breed, int age, Color prevalentColor, string behavior, AnimalType animalType, string pictureUrl)
        {
            var animal = new Animal(name, breed, age, prevalentColor, behavior, animalType, pictureUrl);

            if (!IsAValidAnimal(animal)) return;

            Insert(animal);
        }

        public void UpdateAnimal(Animal animal)
        {
            if (!IsAValidAnimal(animal)) return;

            Update(animal);
        }

        private bool IsAValidAnimal(IValidation animal)
        {
            if (animal.IsValid()) return true;

            RaiseNotification(animal.ValidationResult, NotificationType.Error);
            return false;
        }
    }
}
