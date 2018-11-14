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
    [Route("animals")]
    public class AnimalController : BaseController
    {
        private readonly IAnimalService _animalService;
        private readonly IAdopterService _adopterService;
        private readonly ILogger _logger;

        public AnimalController(IAnimalService animalService, IAdopterService adopterService, IMediatorBus mediator, INotificationHandler<DomainNotification> notifications, ILoggerFactory loggerFactory, IUser user) : base(mediator, notifications, loggerFactory, user)
        {
            _animalService = animalService;
            _adopterService = adopterService;
            _logger = loggerFactory.CreateLogger("Error");
        }

        [HttpGet]
        [AuthorizeRoles(Roles.ADMIN, Roles.ADOPTER)]
        [SwaggerOperation(Summary = "Animal Paged List", Tags = new[] { "Animals" })]
        public IActionResult AnimalPagedList([FromQuery] AnimalFilter filter)
        {
            try
            {
                var result = new PagingResultViewModel<AnimalListViewModel>();

                var pagingAnimals = _animalService.GetAnimals(filter.Page.Value, filter.PageSize.Value, filter.AnimalStatus, filter.Search, filter.IsActive);

                if (pagingAnimals.Elements.Any())
                {
                    var animalsListViewModel = pagingAnimals.Elements.Select(animal => new AnimalListViewModel()
                    {
                        Id = animal.Id,
                        Name = animal.Name,
                        Age = animal.Age,
                        AnimalType = animal.AnimalType.GetDescription(),
                        Behavior = animal.Behavior,
                        Breed = animal.Breed,
                        Status = animal.Status,
                        AdopterName = animal.Adopter?.Name,
                        PictureUrl = animal.PictureUrl
                    });

                    result = new PagingResultViewModel<AnimalListViewModel>(filter.Page.Value, filter.PageSize.Value)
                    {
                        Elements = animalsListViewModel,
                        ElementsCount = pagingAnimals.ElementsCount
                    };
                }
                else
                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/animals", result: "No content");

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.OK, result: result)
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/animals", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao listar os animais");
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Animal By Id", Tags = new[] { "Animals" })]
        public IActionResult AnimalById(Guid id)
        {
            try
            {
                var animal = _animalService.FindById(id);

                var message = $"O animal com Id {id} não existe";

                if (animal == null)
                {
                    var error = JsonConvert.SerializeObject(message);
                    _logger.LogError(error);

                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/animals", isError: true, result: message);
                }

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.OK, result: animal)
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/animals", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao obter o animal");
            }
        }

        [HttpPost]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Insert Animal", Tags = new[] { "Animals" })]
        public IActionResult InsertAnimal([FromBody] AnimalViewModel animalViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyInvalidModelError();
                    return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Os dados fornecidos são inválidos");
                }
                
                _animalService.InsertAnimal(animalViewModel.Name, animalViewModel.Breed, animalViewModel.Age, animalViewModel.PrevalentColor, animalViewModel.Behavior, animalViewModel.AnimalType, animalViewModel.PictureUrl);

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.Created, "hipets/api/v1/animals")
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/animals", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao inserir o animal");
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Update Animal", Tags = new[] { "Animals" })]
        public IActionResult UpdateAnimal(Guid id, [FromBody] AnimalViewModel animalViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyInvalidModelError();
                    return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Os dados fornecidos são inválidos");
                }

                var animal = _animalService.FindById(id);

                var message = $"O animal com Id {id} não existe";

                if (animal == null)
                {
                    var error = JsonConvert.SerializeObject(message);
                    _logger.LogError(error);

                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/animals", isError: true, result: message);
                }

                animal.UpdateAnimal(animalViewModel.Name, animalViewModel.Breed, animalViewModel.Age, animalViewModel.PrevalentColor, animalViewModel.Behavior, animalViewModel.AnimalType, animalViewModel.PictureUrl);

                _animalService.UpdateAnimal(animal);

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.OK)
                    : RequestResponse(HttpStatusCode.Conflict, isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao atualizar o animal");
            }
        }
        
        [HttpDelete]
        [Route("{id:Guid}")]
        [AuthorizeRoles(Roles.ADMIN)]
        [SwaggerOperation(Summary = "Delete Animal", Tags = new[] { "Animals" })]
        public IActionResult DeleteAnimal(Guid id)
        {
            try
            {
                var animal = _animalService.FindById(id);

                var message = $"O animal com Id {id} não existe";

                if (animal == null)
                {
                    var error = JsonConvert.SerializeObject(message);
                    _logger.LogError(error);

                    return RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/animals", isError: true, result: message);
                }
                                
                _animalService.Delete(animal);

                return IsAValidOperation()
                    ? RequestResponse(HttpStatusCode.NoContent)
                    : RequestResponse(HttpStatusCode.NotFound, "hipets/api/v1/animals", isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao remover o animal");
            }
        }
    }
}
