using HiPets.Domain.Entities;
using HiPets.Domain.Interfaces;
using HiPets.Infra.Data.Contexts;
using System.Linq;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;

namespace HiPets.Infra.Data.Repositories
{
    public sealed class AdopterRepository : Repository<Adopter>, IAdopterRepository
    {
        public AdopterRepository(Context context, IUser user) : base(context, user)
        {
        }
        
        public PagingResult<Adopter> GetAdopters(int page, int pageSize, string searchString = null, Status? status = null)
        {
            var query = DbSet
                .Where(adopter => adopter.Status != Status.Deleted);

            if (status != null)
                query = query.Where(adopter => adopter.Status == status);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(adopter => adopter.Name.ToLower().Contains(searchString.ToLower()));
            }

            return new PagingResult<Adopter>()
            {
                Elements = query.OrderByDescending(adopter => adopter.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize),
                ElementsCount = query.Count()
            };
        }
    }
}
