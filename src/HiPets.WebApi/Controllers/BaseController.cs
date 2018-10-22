using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using HiPets.Domain.Notifications;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Helpers;

namespace HiPets.WebApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseController : Controller
    {
        protected DomainNotificationHandler Notifications { get; }
        protected ILogger Logger { get; }
        protected IMediatorHandler Mediator { get; }

        protected BaseController(IMediatorHandler mediator, INotificationHandler<DomainNotification> notifications, ILoggerFactory loggerFactory, IUser user)
        {
            Mediator = mediator;
            Notifications = (DomainNotificationHandler)notifications;
            Logger = loggerFactory.CreateLogger("Error");
        }

        protected IActionResult RequestResponse(HttpStatusCode httpStatusCode, string uri = null, bool isError = false, object result = null)
        {
            if (IsAValidOperation() && !isError)
            {
                if (httpStatusCode == HttpStatusCode.Created)
                    return Created(uri, new
                    {
                        success = true,
                        data = result,
                        notifications = Notifications.GetNotifications(NotificationType.Info)
                    });

                if (httpStatusCode == HttpStatusCode.NotFound)
                    return Created(uri, new
                    {
                        success = true,
                        data = result
                    });

                if (httpStatusCode == HttpStatusCode.NoContent)
                    return Created(uri, new
                    {
                        success = true,
                        data = result
                    });

                return Ok(new
                {
                    success = true,
                    data = result,
                    notifications = Notifications.GetNotifications(NotificationType.Info)
                });
            }

            switch (httpStatusCode)
            {
                case HttpStatusCode.UnprocessableEntity:
                    return UnprocessableEntity(new
                    {
                        success = false,
                        errors = result ?? Notifications.GetNotifications(NotificationType.Error)
                    });
                case HttpStatusCode.NotFound:
                    return NotFound(new
                    {
                        success = false,
                        errors = result ?? Notifications.GetNotifications(NotificationType.Error)
                    });
                case HttpStatusCode.Conflict:
                    return NotFound(new
                    {
                        success = false,
                        errors = result ?? Notifications.GetNotifications(NotificationType.Error)
                    });
                case HttpStatusCode.Unauthorized:
                    return Unauthorized();
                case HttpStatusCode.InternalServerError:
                    return new InternalServerError();
                default:
                    return BadRequest(new
                    {
                        success = false,
                        errors = result ?? Notifications.GetNotifications(NotificationType.Error)
                    });
            }
        }

        protected bool IsAValidOperation()
        {
            return (!Notifications.HasNotifications(NotificationType.Error));
        }

        protected void NotifyInvalidModelError()
        {
            var errorList = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            foreach (var error in errorList)
            {
                var erroMsg = error.Value.FirstOrDefault();
                NotifyError(error.Key, erroMsg?.ToString());
            }
        }

        protected void NotifyError(string code, string message)
        {
            Mediator.RaiseNotification(new DomainNotification(code, message, NotificationType.Error));
        }

        public class InternalServerError : ObjectResult
        {
            public InternalServerError(object value) : base(value)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }

            public InternalServerError() : this(null)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}
