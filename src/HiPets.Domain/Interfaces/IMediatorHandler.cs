using HiPets.Domain.Notifications;
using System.Threading.Tasks;

namespace HiPets.Domain.Interfaces
{
    public interface IMediatorHandler
    {
        Task RaiseNotification(DomainNotification e);
    }
}
