using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using HiPets.QueryStack.DataModels;
using System;

namespace HiPets.QueryStack.Repositories
{
    public interface IAdoptionRepository
    {
        PagingResult<AdoptionModel> GetAdoptions(Guid? userId, int page, int pageSize, AdoptionStatus? adoptionStatus = null, string searchString = null, Status? status = null);
    }
}
