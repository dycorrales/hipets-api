using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using HiPets.Domain.Interfaces;

namespace HiPets.Application.Services
{
    public sealed class AdopterService : Service<Adopter>, IAdopterService
    {
        private readonly IAdopterRepository _repository;

        public AdopterService(IMediatorBus mediator, IUnitOfWork unitOfWork, IAdopterRepository repository) : base(mediator, unitOfWork, repository)
        {
            _repository = repository;
        }

        public PagingResult<Adopter> GetAdopters(int page, int pageSize, string searchString = null, bool? isActive = null)
        {
            Status? status = null;

            if (isActive != null)
            {
                status = isActive.Value ? Status.Active : Status.Inactive;
            }

            return _repository.GetAdopters(page, pageSize, searchString, status);
        }

        public void InsertAdopter(string name, string phoneNumber, string email)
        {
            var adopter = new Adopter(name, phoneNumber, email);

            if (!IsAValidAdopter(adopter)) return;

            Insert(adopter);
        }

        public void UpdateAdopter(Adopter adopter)
        {
            if (!IsAValidAdopter(adopter)) return;

            Update(adopter);
        }

        private bool IsAValidAdopter(IValidation adopter)
        {
            if (adopter.IsValid()) return true;

            RaiseNotification(adopter.ValidationResult, NotificationType.Error);
            return false;
        }
    }
}
