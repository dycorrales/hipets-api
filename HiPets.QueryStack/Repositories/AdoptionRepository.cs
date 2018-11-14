using HiPets.Domain.Entities;
using HiPets.Domain.Interfaces;
using System.Linq;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using HiPets.QueryStack.DataModels;

namespace HiPets.QueryStack.Repositories
{
    public sealed class AdoptionRepository : IAdoptionRepository
    {
        private readonly IConfigurationRoot _configurationRoot;

        public AdoptionRepository(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }
        
        public PagingResult<AdoptionModel> GetAdoptions(Guid? userId, int page, int pageSize, AdoptionStatus? adoptionStatus = null, string searchString = null, Status? status = null)
        {
            using (var connection = new SqlConnection(_configurationRoot.GetConnectionString("HiPetsAdoptionsConnectionString")))
            {
                var conditionOperator = "AND";

                var select = $"SELECT  id, createdAt, animalId, animalName, animalBreed, animalPictureUrl, animalType, adopterId, adopterName, adopterPhoneNumber, " +
                             $" adopterEmail, adoptionStatus, adoptionObservation" +
                             $" FROM Adoptions";

                if (userId != null)
                    select = select + $" WHERE adopterId = '{userId}'";
                else
                    conditionOperator = " WHERE";


                if (!string.IsNullOrEmpty(searchString))
                {
                    select = select + $" {conditionOperator} (animalName like %{searchString}% OR adopterName like %{searchString}%)";
                    conditionOperator = "AND";
                }

                if (adoptionStatus != null)
                    select = select + $" {conditionOperator} adoptionStatus = {(int?)adoptionStatus}";

                select = select + $" ORDER BY CreatedAt Desc OFFSET {(page - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

                var result = connection.Query<AdoptionModel>(select);

                return new PagingResult<AdoptionModel>()
                {
                    Elements = result,
                    ElementsCount = result.Count()
                };
            }
        }
    }
}
