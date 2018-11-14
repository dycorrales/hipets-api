using HiPets.Domain.Commands;
using HiPets.Domain.Events;
using HiPets.Domain.Notifications;
using MediatR;
using System.Threading.Tasks;

namespace HiPets.Domain.Interfaces
{
    public interface IMediatorBus
    {
        Task RaiseNotification(DomainNotification e);
        Task SendCommand<TEntity>(TEntity command) where TEntity : Command;
        Task RaiseEvent<TEntity>(TEntity e) where TEntity : Event;
    }
}
