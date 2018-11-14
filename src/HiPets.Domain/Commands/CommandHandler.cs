using FluentValidation.Results;
using HiPets.Domain.Helpers;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Notifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiPets.Domain.Commands
{
    public abstract class CommandHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IMediatorBus _mediator;
        private readonly DomainNotificationHandler _notifications;

        protected CommandHandler(IUnitOfWork uow, IMediatorBus mediator, INotificationHandler<DomainNotification> notifications)
        {
            _uow = uow;
            _mediator = mediator;
            _notifications = (DomainNotificationHandler)notifications;
        }

        protected void ErrorNotifications(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                _mediator.RaiseNotification(new DomainNotification(error.PropertyName, error.ErrorMessage, NotificationType.Error));
            }
        }

        protected bool Commit()
        {
            if (_notifications.HasNotifications(NotificationType.Error)) return false;
            var commandResponse = _uow.Commit();
            return commandResponse.Success;
        }
    }
}
