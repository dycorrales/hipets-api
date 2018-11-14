using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using HiPets.Domain.Interfaces;
using System;

namespace HiPets.Application.Services
{
    public sealed class AdoptionService : Service<Adoption>, IAdoptionService
    {
        private readonly IAdoptionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAnimalService _animalService;

        public AdoptionService(IAnimalService animalService, IMediatorBus mediator, IUnitOfWork unitOfWork, IAdoptionRepository repository) : base(mediator, unitOfWork, repository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _animalService = animalService;
        }

        public void InsertAdoption(Guid animalId, Guid adopterId)
        {
            var adoption = new Adoption(animalId, adopterId);

            var animal = _animalService.FindById(animalId);

            if(animal == null)
            {
                RaiseNotification(nameof(Animal), "Animal para adoção não existe", NotificationType.Error);
                return;
            }

            animal.InAdoptionProccess();

            _repository.Update(animal);

            if (!IsAValidAdoption(adoption)) return;

            Insert(adoption);
        }

        public override void Update(Adoption adoption)
        {
            if (!IsAValidAdoption(adoption)) return;
            
            _repository.Update(adoption);

            if(adoption.AdoptionStatus == AdoptionStatus.Accepted || adoption.AdoptionStatus == AdoptionStatus.Rejected)
            {
                var animal = _animalService.FindById(adoption.AnimalId);

                if(animal == null)
                {
                    RaiseNotification(nameof(Animal), "Animal para adoção não existe", NotificationType.Error);
                    return;
                }

                if (adoption.AdoptionStatus == AdoptionStatus.Accepted)
                    animal.Adopt(adoption.AdopterId);
                else
                    animal.ReturnForAdoption();

                _repository.Update(animal);
            }

            var response = _unitOfWork.Commit();

            if (!response.Success)
            {
                RaiseNotification(nameof(Adoption), "Erro ao salvar os dados", NotificationType.Error);
                return;
            }

            RaiseNotification(nameof(Adoption), "Dados salvados com sucesso", NotificationType.Info);
        }

        private bool IsAValidAdoption(IValidation adoption)
        {
            if (adoption.IsValid()) return true;

            RaiseNotification(adoption.ValidationResult, NotificationType.Error);
            return false;
        }
    }
}
