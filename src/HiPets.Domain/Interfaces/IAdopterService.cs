using HiPets.Domain.Entities;
using HiPets.Domain.Helpers.Utils;

namespace HiPets.Domain.Interfaces
{
    public interface IAdopterService : IService<Adopter>
    {
        void InsertAdopter(string name, string phoneNumber, string email);
        void UpdateAdopter(Adopter adopter);

        PagingResult<Adopter> GetAdopters(int page, int pageSize, string searchString = null, bool? isActive = null);
    }
}
