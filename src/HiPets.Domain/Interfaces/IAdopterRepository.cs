using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;

namespace HiPets.Domain.Interfaces
{
    public interface IAdopterRepository : IRepository<Adopter>
    {
        PagingResult<Adopter> GetAdopters(int page, int pageSize, string searchString = null, Status? status = null);
    }
}
