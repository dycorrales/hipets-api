using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;

namespace HiPets.Domain.Interfaces
{
    public interface IAnimalRepository : IRepository<Animal>
    {
        PagingResult<Animal> GetAnimals(int page, int pageSize, AnimalStatus animalStatus, string searchString = null, Status? status = null);
    }
}
