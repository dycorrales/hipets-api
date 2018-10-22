using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiPets.Domain.Helpers;
using MediatR;

namespace HiPets.Domain.Notifications
{
    public class DomainNotificationHandler : INotificationHandler<DomainNotification>
    {
        private List<DomainNotification> _notifications;

        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotification>();
        }

        public List<DomainNotification> GetNotifications()
        {
            return _notifications.ToList();
        }

        public List<DomainNotification> GetNotifications(NotificationType type)
        {
            return _notifications.Where(n => n.Type == type).ToList();
        }

        public bool HasNotifications(NotificationType type)
        {
            return _notifications.Any(n => n.Type == type);
        }

        public void Dispose()
        {
            _notifications = new List<DomainNotification>();
        }

        public Task Handle(DomainNotification message, CancellationToken cancellationToken)
        {
            _notifications.Add(message);
            return Task.CompletedTask;
        }
    }
}
