using HiPets.Domain.Entities;
using HiPets.Domain.Interfaces;
using HiPets.Infra.Data.Contexts;
using System.Linq;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using Microsoft.EntityFrameworkCore;

namespace HiPets.Infra.Data.Repositories
{
    public sealed class AnimalRepository : Repository<Animal>, IAnimalRepository
    {
        public AnimalRepository(Context context, IUser user) : base(context, user)
        {
        }
        
        public PagingResult<Animal> GetAnimals(int page, int pageSize, AnimalStatus animalStatus, string searchString = null, Status? status = null)
        {
            var query = DbSet
                .Include(animal => animal.Adopter)
                .Where(animal => animal.Status != Status.Deleted && animal.AnimalStatus == animalStatus);

            if (animalStatus == AnimalStatus.Adopted && !User.IsAdminUser)
                query = query.Where(animal => animal.AdopterId == User.GetUserId());

            if (status != null)
                query = query.Where(animal => animal.Status == status);
            
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(animal => animal.Name.ToLower().Contains(searchString.ToLower()) || animal.AnimalType.GetDescription().ToLower().Contains(searchString.ToLower()) || animal.Breed.ToLower().Contains(searchString.ToLower()));
            }

            return new PagingResult<Animal>()
            {
                Elements = query.OrderByDescending(animal => animal.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize),
                ElementsCount = query.Count()
            };
        }
    }
}
