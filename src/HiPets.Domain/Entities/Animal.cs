using System;
using HiPets.Domain.Helpers;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Validations;

namespace HiPets.Domain.Entities
{
    public sealed class Animal : Entity, IValidation
    {
        public string Name { get; private set; }
        public string Breed { get; private set; }
        public int Age { get; private set; }
        public Color PrevalentColor { get; private set; }
        public string Behavior { get; private set; }
        public AnimalStatus AnimalStatus { get; private set; }
        public AnimalType AnimalType { get; private set; }
        public int DaysInAdoption { get { return (DateTime.Now - CreatedAt).Days; } }
        public DateTime? AdoptionDate { get; private set; }
        public Guid? AdopterId { get; private set; }
        public Adopter Adopter { get; protected set; }
        public string PictureUrl { get; private set; }

        protected Animal() { }

        public Animal(string name, string breed, int age, Color prevalentColor, string behavior, AnimalType animalType, string pictureUrl) : base(Guid.NewGuid(), Status.Active)
        {
            Name = name;
            Breed = breed;
            Age = age;
            PrevalentColor = prevalentColor;
            Behavior = behavior;
            AnimalStatus = AnimalStatus.ForAdoption;
            AnimalType = animalType;
            PictureUrl = pictureUrl;
        }

        public Animal(Guid id, string name, string breed, int age, Color prevalentColor, string behavior, AnimalType animalType, string pictureUrl) : base(id, Status.Active)
        {
            Name = name;
            Breed = breed;
            Age = age;
            PrevalentColor = prevalentColor;
            Behavior = behavior;
            AnimalStatus = AnimalStatus.ForAdoption;
            AnimalType = animalType;
            PictureUrl = pictureUrl;
        }

        public void UpdateAnimal(string name, string breed, int age, Color prevalentColor, string behavior, AnimalType animalType, string pictureUrl)
        {
            Name = name;
            Breed = breed;
            Age = age;
            PrevalentColor = prevalentColor;
            Behavior = behavior;
            AnimalType = animalType;
            PictureUrl = pictureUrl;
        }

        public void Adopt(Guid adopterId)
        {
            AnimalStatus = AnimalStatus.Adopted;
            AdoptionDate = DateTime.Now;
            AdopterId = adopterId;
        }

        public void InAdoptionProccess()
        {
            AnimalStatus = AnimalStatus.InAdoptionProccess;
        }

        public void ReturnForAdoption()
        {
            AnimalStatus = AnimalStatus.ForAdoption;
        }

        public bool IsForAdoption()
        {
            return AnimalStatus == AnimalStatus.ForAdoption;
        }

        public override bool IsValid()
        {
            var validation = new AnimalValidation();
            validation.ValidateAnimal();
            ValidationResult = validation.Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
