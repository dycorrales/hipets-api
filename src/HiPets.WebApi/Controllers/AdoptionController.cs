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
using HiPets.Domain.Commands;

namespace HiPets.WebApi.Controllers
{
    [Route("adoptions")]
    public class AdoptionController : BaseController
    {
        private readonly IMediatorBus _mediator;
        private readonly IAdoptionService _adoptionService;
        private readonly QueryStack.Repositories.IAdoptionRepository _adoptionRepository;
        private readonly ILogger _logger;
        private readonly IUser _user;

        public AdoptionController(QueryStack.Repositories.IAdoptionRepository adoptionRepository, IAdoptionService adoptionService, IMediatorBus mediator, INotificationHandler<DomainNotification> notifications, ILoggerFactory loggerFactory, IUser user) : base(mediator, notifications, loggerFactory, user)
        {
            _adoptionService = adoptionService;
            _adoptionRepository = adoptionRepository;
            _mediator = mediator;
            _user = user;
            _logger = loggerFactory.CreateLogger("Error");
        }

        [HttpGet]
        [AuthorizeRoles(Roles.ADMIN, Roles.ADOPTER)]
        [SwaggerOperation(Summary = "Adoption Paged List", Tags = new[] { "Adoptions" })]
        public IActionResult AdoptionPagedList([FromQuery] AdoptionFilter filter)
        {
            try
            {
                var result = new PagingResultViewModel<AdoptionListViewModel>();

                Status? status = null;

                if (filter.IsActive != null)
                {
                    status = filter.IsActive.Value ? Status.Active : Status.Inactive;
                }

                var userId = _user.IsAdminUser ? null : (Guid?)_user.GetUserId();

                var pagingAdoptions = _adoptionRepository.GetAdoptions(userId, filter.Page.Value, filter.PageSize.Value, filter.AdoptionStatus, filter.Search, status);

                if (pagingAdoptions.Elements.Any())
                {
                    var adoptionsListViewModel = pagingAdoptions.Elements.Select(adoption => new AdoptionListViewModel()
                    {
                        Id = adoption.Id,
                        AnimalName = adoption.AnimalName,
                        AnimalBreed = adoption.AnimalBreed,
                        AnimalType = adoption.AnimalType,
                        AdopterName = adoption.AdopterName,
                        AnimalPictureUrl = adoption.AnimalPictureUrl,
                        AdopterPhoneNumber = adoption.AdopterPhoneNumber,
                        AdopterEmail = adoption.AdopterEmail,
                        AdoptionStatus = adoption.AdoptionStatus.GetDescription(),
                        AdoptionObservation = adoption.AdoptionObservation
                    });

                    result = new PagingResultViewModel<AdoptionListViewModel>(filter.Page.Value, filter.PageSize.Value)
                    {
                        Elements = adoptionsListViewModel,
                        ElementsCount = pagingAdoptions.ElementsCount
                    };
                }
                else
                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adoptions", result: "No content");

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.OK, result: result)
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adoptions", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao listar os animais");
            }
        }
        
        [HttpPost]
        [AuthorizeRoles(Roles.ADOPTER)]
        [SwaggerOperation(Summary = "Request Adoption", Tags = new[] { "Adoptions" })]
        public IActionResult RequestAdoption([FromBody] AdoptionViewModel adoptionViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyInvalidModelError();
                    return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Os dados fornecidos são inválidos");
                }

                //_adoptionService.InsertAdoption(adoptionViewModel.AnimalId, adoptionViewModel.AdopterId);
                _mediator.SendCommand(new RequestAdoption(){
                    AnimalId = adoptionViewModel.AnimalId,
                    AdopterId = adoptionViewModel.AdopterId
                });

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.Created, "hipets/api/v1/adoptions")
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adoptions", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao solicitar adoção");
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Update Adoption Status", Tags = new[] { "Adoptions" })]
        public IActionResult UpdateAdoptionStatus(Guid id, [FromBody] AdoptionStatusViewModel adoptionStatusViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyInvalidModelError();
                    return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Os dados fornecidos são inválidos");
                }

                if(adoptionStatusViewModel.AdoptionStatus == AdoptionStatus.Requested)
                {
                    return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "O status da adoção é inválido");
                }

                var adoption = _adoptionService.FindById(id);

                var message = $"Adoção com Id {id} não existe";

                if (adoption == null)
                {
                    var error = JsonConvert.SerializeObject(message);
                    _logger.LogError(error);

                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adoptions", isError: true, result: message);
                }

                adoption.UpdateAdoptionStatus(adoptionStatusViewModel.AdoptionStatus, adoptionStatusViewModel.AdoptionObservation);

                _adoptionService.Update(adoption);

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.OK)
                    : RequestResponse(HttpStatusCode.Conflict, isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao atualizar adoção");
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Delete Adoption", Tags = new[] { "Adoptions" })]
        public IActionResult DeleteAdoption(Guid id)
        {
            try
            {
                var adoption = _adoptionService.FindById(id);

                var message = $"Adoção com Id {id} não existe";

                if (adoption == null)
                {
                    var error = JsonConvert.SerializeObject(message);
                    _logger.LogError(error);

                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adoptions", isError: true, result: message);
                }

                _adoptionService.Delete(adoption);

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.NoContent)
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/adoptions", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao remover adoção");
            }
        }
    }
}
