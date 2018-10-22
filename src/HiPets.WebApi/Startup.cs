using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using MediatR;
using HiPets.WebApi.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using HiPets.CrossCutting.AspnetFilters;
using Microsoft.EntityFrameworkCore;
using HiPets.WebApi.Helpers;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;
using HiPets.Infra.Data.Contexts;
using HiPets.Infra.Data.Initializers;
using HiPets.CrossCutting.Identity.Data;
using HiPets.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace HiPets.WebApi
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public static IConfiguration Config { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Config = builder.Build();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configurações de Autenticação, Autorização e JWT.
            services.AddMvcSecurity(Configuration);

            var connection = Configuration.GetConnectionString("HiPetsConnectionString");

            services.AddDbContext<Context>(options =>
                options.UseSqlServer(connection)
            );
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(connection));

            services.AddMvc(config =>
            {
                config.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
            })
             .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Options para configurações customizadas
            services.AddOptions();
            services.AddResponseCaching();
            services.AddLogging();

            services.Configure<GzipCompressionProviderOptions>(
                opt => opt.Level = CompressionLevel.Optimal
                );

            services.AddResponseCompression(opt =>
            {
                opt.Providers.Add<GzipCompressionProvider>();
                opt.EnableForHttps = true;
            });

            // MVC com restrição de XML e adição de filtro de ações.
            services.AddMvc(opt =>
            {
                opt.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
                opt.Filters.Add(new ServiceFilterAttribute(typeof(GlobalExceptionHandlingFilter)));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(opt => opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore);

            services.AddCors();

            // Versionamento do WebApi
            services.AddApiVersioning("hipets/api/v{version}");

            // Configurações do Swagger
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
            });
            services.AddSwaggerConfig();

            // MediatR
            services.AddMediatR(typeof(Startup));

            // Registrar todos os DI
            services.AddSingleton(provider => Configuration);
            services.AddDIConfiguration();           
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IHttpContextAccessor accessor, ApplicationContext identityContext, Context context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            loggerFactory.AddLog4Net(Configuration.GetValue<string>("Log4NetConfigFile:Name"));

            loggerFactory.AddDebug();
                       
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            var defaultCulture = new CultureInfo("es-UY");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };

            app.UseRequestLocalization(localizationOptions);

            app.UseCors(c =>
            {
                c.AllowAnyOrigin();
                c.AllowAnyHeader();
                c.AllowAnyMethod();
            });

            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "HiPets API v1.0");
            });

            new IdentityInitializer(userManager, identityContext, roleManager).Initialize();
            var adminUser = identityContext.Users.FirstOrDefaultAsync(u => u.Email == "admin@teste.com").Result;
            var adopterUser = identityContext.Users.FirstOrDefaultAsync(u => u.Email == "js@gmail.com").Result;

            new DataBaseInitializer(context).Initialize(new Guid(adminUser.Id), new Guid(adopterUser.Id));
        }
    }
}
