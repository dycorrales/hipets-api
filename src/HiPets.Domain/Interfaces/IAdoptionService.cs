using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using System;

namespace HiPets.Domain.Interfaces
{
    public interface IAdoptionService : IService<Adoption>
    {
        void InsertAdoption(Guid animalId, Guid adopterId);

        PagingResult<Adoption> GetAdoptions(int page, int pageSize, AdoptionStatus? adoptionStatus = null, string searchString = null, bool? isActive = null);
    }
}
