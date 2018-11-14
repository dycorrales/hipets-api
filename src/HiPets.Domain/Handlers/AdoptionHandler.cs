using HiPets.Domain.Commands;
using HiPets.Domain.Entities;
using HiPets.Domain.Events;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Notifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiPets.Domain.Handlers
{
    public sealed class AdoptionHandler : CommandHandler,
        IRequestHandler<RequestAdoption, string>,
        INotificationHandler<RequestedAdoption>
    {
        private readonly IAdoptionRepository _adoptionRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IAdopterRepository _adopterRepository;
        private readonly IMediatorBus _mediator;

        public AdoptionHandler(IAdoptionRepository adoptionRepository, IAnimalRepository animalRepository, IAdopterRepository adopterRepository, IUnitOfWork uow, IMediatorBus mediator, INotificationHandler<DomainNotification> notifications) : base(uow, mediator, notifications)
        {
            _animalRepository = animalRepository;
            _adoptionRepository = adoptionRepository;
            _adopterRepository = adopterRepository;

            _mediator = mediator;
        }

        public Task<string> Handle(RequestAdoption requestAdoption, CancellationToken cancellationToken)
        {
            var adoption = new Adoption(requestAdoption.AnimalId, requestAdoption.AdopterId);

            var animal = _animalRepository.FindById(requestAdoption.AnimalId);

            if (animal == null)
            {
                _mediator.RaiseNotification(new DomainNotification(nameof(Animal), "Animal para adoção não existe", NotificationType.Error));
                return Task.FromResult("Fail");
            }

            if (animal.AnimalStatus != AnimalStatus.ForAdoption)
            {
                _mediator.RaiseNotification(new DomainNotification(nameof(Animal), "Animal não disponível para adoção", NotificationType.Error));
                return Task.FromResult("Fail");
            }

            animal.InAdoptionProccess();

            _animalRepository.Update(animal);

            if (!IsAValidAdoption(adoption)) return Task.FromResult("Fail");

            _adoptionRepository.Insert(adoption);

            if (Commit())
            {
                var adopter = _adopterRepository.FindById(requestAdoption.AdopterId);

                _mediator.RaiseNotification(new DomainNotification(nameof(Animal), "Solicitude de adoção enviada com sucesso", NotificationType.Info));
                _mediator.RaiseEvent(new RequestedAdoption(adoption.Id)
                {
                    AdopterPhoneNumber = adopter.ContactInfo.PhoneNumber,
                    AdopterEmail = adopter.ContactInfo.Email,
                    AdopterId = adopter.Id,
                    AdopterName = adopter.Name,
                    AdoptionObservation = adoption.Observation,
                    AdoptionStatus = adoption.AdoptionStatus,
                    AnimalBreed = animal.Breed,
                    AnimalId = animal.Id,
                    AnimalName = animal.Name,
                    AnimalPictureUrl = animal.PictureUrl,
                    AnimalType = animal.AnimalType.GetDescription()
                });
            }
            else
                _mediator.RaiseNotification(new DomainNotification(nameof(Animal), "Erro ao solicitar adoção", NotificationType.Error));

            return Task.FromResult("Ok");
        }

        public Task Handle(RequestedAdoption requestedAdoption, CancellationToken cancellationToken)
        {
            var result = _adoptionRepository.InsertAdoptionRequested(requestedAdoption.Id, requestedAdoption.CreationDate, requestedAdoption.AnimalId, requestedAdoption.AnimalName, requestedAdoption.AnimalBreed, requestedAdoption.AnimalPictureUrl, requestedAdoption.AnimalType, requestedAdoption.AdopterId, requestedAdoption.AdopterName, requestedAdoption.AdopterPhoneNumber, requestedAdoption.AdopterEmail, requestedAdoption.AdoptionObservation, requestedAdoption.AdoptionStatus);
                       
            return Task.CompletedTask;
        }

        private bool IsAValidAdoption(IValidation adoption)
        {
            if (adoption.IsValid()) return true;

            _mediator.RaiseNotification(new DomainNotification(adoption.ValidationResult.Errors[0].PropertyName, adoption.ValidationResult.Errors[0].ErrorMessage, NotificationType.Error));
            return false;
        }
    }
}
