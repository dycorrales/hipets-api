using System.Threading.Tasks;
using MediatR;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Notifications;

namespace HiPets.Application.Mediator
{
    public class MediatorHandler : IMediatorHandler
    { 
        private readonly IMediator _mediator;

        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task RaiseNotification(DomainNotification notification)
        {
            await _mediator.Publish(notification);
        }
    }
}
