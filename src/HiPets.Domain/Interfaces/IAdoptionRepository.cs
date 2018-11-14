using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using System;

namespace HiPets.Domain.Interfaces
{
    public interface IAdoptionRepository : IRepository<Adoption>
    {
        int InsertAdoptionRequested(Guid id, DateTime createdAt, Guid animalId, string animalName, string animalBreed, string animalPictureUrl, string animalType, Guid adopterId, string adopterName, string adopterPhoneNumber, string adopterEmail, string adoptionObservation, AdoptionStatus adoptionStatus);
    }
}
