using HiPets.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiPets.QueryStack.DataModels
{
    public class AdoptionModel
    {
        public Guid Id { get; }
        public Guid AnimalId { get; set; }
        public string AnimalName { get; set; }
        public string AnimalBreed { get; set; }
        public string AnimalPictureUrl { get; set; }
        public string AnimalType { get; set; }
        public Guid AdopterId { get; set; }
        public string AdopterName { get; set; }
        public string AdopterPhoneNumber { get; set; }
        public string AdopterEmail { get; set; }
        public string AdoptionObservation { get; set; }
        public AdoptionStatus AdoptionStatus { get; set; }
    }
}
