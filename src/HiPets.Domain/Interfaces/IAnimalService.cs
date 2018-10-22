using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;

namespace HiPets.Domain.Interfaces
{
    public interface IAnimalService : IService<Animal>
    {
        void InsertAnimal(string name, string breed, int age, Color prevalentColor, string behavior, AnimalType animalType, string pictureUrl);
        void UpdateAnimal(Animal animal);

        PagingResult<Animal> GetAnimals(int page, int pageSize, AnimalStatus animalStatus, string searchString = null, bool? isActive = null);
    }
}
