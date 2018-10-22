using Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Notifications;
using HiPets.WebApi.ViewModels;
using HiPets.Domain.Helpers.Utils;
using System.Net;
using Newtonsoft.Json;
using HiPets.WebApi.Helpers;
using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.WebApi.Filters;
using HiPets.CrossCutting.Identity.Models;

namespace HiPets.WebApi.Controllers
{
    [Route("adopters")]
    public class AdopterController : BaseController
    {
        private readonly IAdopterService _adopterService;
        private readonly ILogger _logger;

        public AdopterController(IAdopterService adopterService, IMediatorHandler mediator, INotificationHandler<DomainNotification> notifications, ILoggerFactory loggerFactory, IUser user) : base(mediator, notifications, loggerFactory, user)
        {
            _adopterService = adopterService;
            _logger = loggerFactory.CreateLogger("Error");
        }

        [HttpGet]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Adopter Paged List", Tags = new[] { "Adopters" })]
        public IActionResult AdopterPagedList([FromQuery] CustomFilter filter)
        {
            try
            {
                var result = new PagingResultViewModel<AdopterListViewModel>();

                var pagingAdopters = _adopterService.GetAdopters(filter.Page.Value, filter.PageSize.Value, filter.Search, filter.IsActive);

                if (pagingAdopters.Elements.Any())
                {
                    var animalsListViewModel = pagingAdopters.Elements.Select(adopter => new AdopterListViewModel()
                    {
                        Id = adopter.Id,
                        Name = adopter.Name,
                        PhoneNumber = adopter.ContactInfo.FormatPhoneNumber,
                        Email = adopter.ContactInfo.Email,
                        Status = adopter.Status
                    });

                    result = new PagingResultViewModel<AdopterListViewModel>(filter.Page.Value, filter.PageSize.Value)
                    {
                        Elements = animalsListViewModel,
                        ElementsCount = pagingAdopters.ElementsCount
                    };
                }
                else
                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adopters", result: "No content");

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.OK, result: result)
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adopters", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao listar os clientes");
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Adopter By Id", Tags = new[] { "Adopters" })]
        public IActionResult AdopterById(Guid id)
        {
            try
            {
                var adopter = _adopterService.FindById(id);

                var message = $"O cliente com Id {id} não existe";

                if (adopter == null)
                {
                    var error = JsonConvert.SerializeObject(message);
                    _logger.LogError(error);

                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adopters", isError: true, result: message);
                }

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.OK, result: adopter)
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adopters", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao obter o cliente");
            }
        }

        [HttpPost]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Insert Adopter", Tags = new[] { "Adopters" })]
        public IActionResult InsertAdopter([FromBody] AdopterViewModel adopterViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyInvalidModelError();
                    return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Os dados fornecidos são inválidos");
                }

                _adopterService.InsertAdopter(adopterViewModel.Name, adopterViewModel.PhoneNumber, adopterViewModel.Email);

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.Created, "hipets/api/v1/adopters")
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adopters", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao inserir o cliente");
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Update Adopter", Tags = new[] { "Adopters" })]
        public IActionResult UpdateAdopter(Guid id, [FromBody] AdopterViewModel adopterViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyInvalidModelError();
                    return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Os dados fornecidos são inválidos");
                }

                var adopter = _adopterService.FindById(id);

                var message = $"O cliente com Id {id} não existe";

                if (adopter == null)
                {
                    var error = JsonConvert.SerializeObject(message);
                    _logger.LogError(error);

                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adopters", isError: true, result: message);
                }

                adopter.UpdateAdopter(adopterViewModel.Name, adopterViewModel.PhoneNumber, adopterViewModel.Email);

                _adopterService.UpdateAdopter(adopter);

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.OK)
                    : RequestResponse(HttpStatusCode.Conflict, isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao atualizar o cliente");
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Delete Adopter", Tags = new[] { "Adopters" })]
        public IActionResult DeleteAdopter(Guid id)
        {
            try
            {
                var adopter = _adopterService.FindById(id);

                var message = $"O cliente com Id {id} não existe";

                if (adopter == null)
                {
                    var error = JsonConvert.SerializeObject(message);
                    _logger.LogError(error);

                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adopters", isError: true, result: message);
                }

                _adopterService.Delete(adopter);

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.NoContent)
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adopters", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao remover o cliente");
            }
        }
    }
}
