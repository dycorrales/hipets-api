using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;

namespace HiPets.Domain.Interfaces
{
    public interface IAdoptionRepository : IRepository<Adoption>
    {
        PagingResult<Adoption> GetAdoptions(int page, int pageSize, AdoptionStatus? adoptionStatus = null, string searchString = null, Status? status = null);
    }
}
