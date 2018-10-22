using Microsoft.Extensions.DependencyInjection;
using HiPets.CrossCutting.IoC;

namespace HiPets.WebApi.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            DependencyInjectionResolver.RegisterServices(services);
        }
    }
}
