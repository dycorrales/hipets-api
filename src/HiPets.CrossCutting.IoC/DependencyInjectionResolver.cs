using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;
using HiPets.Domain.Interfaces;
using HiPets.Application.Mediator;
using HiPets.Domain.Notifications;
using HiPets.CrossCutting.AspnetFilters;
using HiPets.Application.Services;
using HiPets.Infra.Data.Repositories;
using HiPets.Infra.Data.UnitOfWorks;
using HiPets.CrossCutting.Identity.Models;
using HiPets.CrossCutting.Identity.Data;

namespace HiPets.CrossCutting.IoC
{
    public class DependencyInjectionResolver
    {
        public static void RegisterServices(IServiceCollection services)
        {  // ASPNET
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Domain 
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddScoped<IAnimalService, AnimalService>();
            services.AddScoped<IAnimalRepository, AnimalRepository>();
            services.AddScoped<IAdopterService, AdopterService>();
            services.AddScoped<IAdopterRepository, AdopterRepository>();
            services.AddScoped<IAdoptionService, AdoptionService>();
            services.AddScoped<IAdoptionRepository, AdoptionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            // Infra - Identity
            services.AddScoped<IUser, AspNetUser>();

            // Infra - Filtros
            services.AddScoped<ILogger<GlobalExceptionHandlingFilter>, Logger<GlobalExceptionHandlingFilter>>();
            services.AddScoped<ILogger<GlobalActionLogger>, Logger<GlobalActionLogger>>();
            services.AddScoped<GlobalExceptionHandlingFilter>();
            services.AddScoped<GlobalActionLogger>();
        }
    }
}
