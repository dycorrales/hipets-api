using HiPets.Domain.Entities;
using HiPets.Domain.Interfaces;
using HiPets.Infra.Data.Contexts;
using System.Linq;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using Microsoft.EntityFrameworkCore;

namespace HiPets.Infra.Data.Repositories
{
    public sealed class AdoptionRepository : Repository<Adoption>, IAdoptionRepository
    {
        public AdoptionRepository(Context context, IUser user) : base(context, user)
        {
        }
        
        public PagingResult<Adoption> GetAdoptions(int page, int pageSize, AdoptionStatus? adoptionStatus = null, string searchString = null, Status? status = null)
        {
            var query = DbSet
                .Include(adoption => adoption.Adopter)
                .Include(adoption => adoption.Animal)
                .Where(adoption => adoption.Status != Status.Deleted);

            if (status != null)
                query = query.Where(adoption => adoption.Status == status);

            if (adoptionStatus != null)
                query = query.Where(adoption => adoption.AdoptionStatus == adoptionStatus);

            if (!User.IsAdminUser)
                query = query.Where(adoption => adoption.AdopterId == User.GetUserId());

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(adoption => adoption.Animal.Name.ToLower().Contains(searchString.ToLower()) || adoption.Adopter.Name.ToLower().Contains(searchString.ToLower()));
            }

            return new PagingResult<Adoption>()
            {
                Elements = query.OrderByDescending(adoption => adoption.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize),
                ElementsCount = query.Count()
            };
        }
    }
}
