using HiPets.Domain.Entities;
using HiPets.Domain.Interfaces;
using HiPets.Infra.Data.Contexts;
using System.Linq;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HiPets.Infra.Data.Repositories
{
    public sealed class AdoptionRepository : Repository<Adoption>, IAdoptionRepository
    {
        private readonly IConfigurationRoot _configurationRoot;

        public AdoptionRepository(IConfigurationRoot configurationRoot, Context context, IUser user) : base(context, user)
        {
            _configurationRoot = configurationRoot;
        }

        public int InsertAdoptionRequested(Guid id, DateTime createdAt, Guid animalId, string animalName, string animalBreed, string animalPictureUrl, string animalType, Guid adopterId, string adopterName, string adopterPhoneNumber, string adopterEmail, string adoptionObservation, AdoptionStatus adoptionStatus)
        {
            using (var connection = new SqlConnection(_configurationRoot.GetConnectionString("HiPetsAdoptionsConnectionString")))
            {
                var insert = $"Insert Into Adoptions(id, createdAt, animalId, animalName, animalBreed, animalPictureUrl, animalType, adopterId, adopterName, adopterPhoneNumber, adopterEmail, adoptionStatus, adoptionObservation) values('{id}', '{createdAt}', '{animalId}', '{animalName}', '{animalBreed}', '{animalPictureUrl}', '{animalType}', '{adopterId}', '{adopterName}', '{adopterPhoneNumber}', '{adopterEmail}', {(int)adoptionStatus}, '{adoptionObservation}')";

                return connection.Execute(insert);
            }
        }
    }
}
