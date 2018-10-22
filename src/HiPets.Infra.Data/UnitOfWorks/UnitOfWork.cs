using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Helpers;
using HiPets.Domain.Notifications;
using HiPets.Infra.Data.Contexts;

namespace HiPets.Infra.Data.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly ILogger _logger;

        public UnitOfWork(Context context, IMediatorHandler mediatorHandler, ILoggerFactory logger)
        {
            _context = context;
            _mediatorHandler = mediatorHandler;
            _logger = logger.CreateLogger<UnitOfWork>();
        }

        public Response Commit()
        {
            try
            {
                return _context.SaveChanges() > 0 ? Response.Ok : Response.Fail;
            }
            catch (DbUpdateException ex)
            {
                var decodedErrors = EfExceptionHelper.TryDecodeDbUpdateException(ex);

                if (decodedErrors == null)
                {
                    _logger.LogError($"On saving in database. {DateTime.Today}");
                    _mediatorHandler.RaiseNotification(new DomainNotification("Error", ex.Message, NotificationType.Error));
                }

                if (decodedErrors == null) return Response.Fail;

                foreach (var entityValidationErro in decodedErrors)
                {
                    _logger.LogError($"{entityValidationErro.ErrorMessage}. {DateTime.Today}");
                    _mediatorHandler.RaiseNotification(new DomainNotification("Error", entityValidationErro.ErrorMessage, NotificationType.Error));
                }

                return Response.Fail;
            }
            catch (Exception ex)
            {
                _context.Dispose();
                _logger.LogError($"On saving in database. {DateTime.Today}");
                _mediatorHandler.RaiseNotification(new DomainNotification("Error",  ex.Message, NotificationType.Error));

                return Response.Fail;
            }
        }
    }

    public static class EfExceptionHelper
    {
        public static List<ValidationResult> Errors;

        /// <summary>
        /// If there are no errors then it is valid
        /// </summary>
        public static bool IsValid => Errors == null;

        public static IReadOnlyList<ValidationResult> EfErrors => Errors ?? new List<ValidationResult>();

        public static IEnumerable<ValidationResult> SetErrors(IEnumerable<ValidationResult> errors)
        {
            return errors.ToList();
        }

        private static readonly Dictionary<int, string> SqlErrorTextDict =
        new Dictionary<int, string>
        {
            { 547, "This operation failed because another data entry uses this input" },
            { 2601, "One of the properties is marked as unique index and an entry with this value already exists" },
            { 2627, "Can not insert duplicate key in object" }
        };

        public static IEnumerable<ValidationResult> TryDecodeDbUpdateException(DbUpdateException ex)
        {
            if (!(ex.InnerException is DbUpdateException) ||
                !(ex.InnerException.InnerException is SqlException))
                return null;
            var sqlException =
                (SqlException)ex.InnerException.InnerException;
            var result = new List<ValidationResult>();
            for (var i = 0; i < sqlException.Errors.Count; i++)
            {
                var errorNum = sqlException.Errors[i].Number;
                if (SqlErrorTextDict.TryGetValue(errorNum, out string errorText))
                    result.Add(new ValidationResult(errorText));
            }
            return result.Any() ? result : null;
        }
    }
}
