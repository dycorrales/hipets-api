using System.Threading.Tasks;
using MediatR;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Notifications;
using HiPets.Domain.Commands;
using HiPets.Domain.Events;
using System.Threading;

namespace HiPets.Domain.Bus
{
    public class MediatorBus : IMediatorBus
    { 
        private readonly IMediator _mediator;

        public MediatorBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task RaiseNotification(DomainNotification notification)
        {
            await _mediator.Publish(notification);
        }

        public async Task SendCommand<TEntity>(TEntity command) where TEntity : Command
        {
            await _mediator.Send(command);
        }

        public async Task RaiseEvent<TEntity>(TEntity e) where TEntity : Event
        {
            await _mediator.Publish(e);
        }
    }
}
