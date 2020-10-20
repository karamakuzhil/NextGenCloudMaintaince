using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Device.Application.Device.Commands.RegisterDevice;
using Device.Application.Device.Queries.FindDevice;
using Device.Application.IntegrationEvents.Events;
using Device.Persistance;
using Device.Persistance.Repositories;
using Devices.Api.Controllers;
using Devices.Domain.Interfaces;
using EventBusServiceBus;
using EventDrivers;
using EventDrivers.Abstractions;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace Devices.Api
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services
            services.AddDbContext<DeviceDbContext>(options =>
                   options.UseSqlServer(_configuration.GetConnectionString("DeviceDatabase"), x => x.MigrationsAssembly("Device.Persistance")));

            services
                .AddIntegrationServices(_configuration)
                .RegisterEventBus(_configuration)
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
                

            //Add Repos
            services.AddScoped<IDeviceRespository, DeviceRepository>();

            // Add MediatR and handlers
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // if you have handlers/events in other assemblies
            services.AddMediatR(typeof(RegisterDeviceCommandHandler).Assembly);
            services.AddMediatR(typeof(FindDeviceQueryHandler).Assembly);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DeviceManagement API", Version = "v1", Description = "The DeviceManagement Service HTTP API" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            SetupAutoMapper();
            ConfigureEventBus(app);

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeviceManagement API - v1");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }

        //Subscribe to Events
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<DeviceRegistrationIntegrationEvent, DeviceRegistrationIntegrationEventHandler>();

        }


        private void SetupAutoMapper()
        {
            // setup automapper
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Devices.Domain.AggregatesModel.Device, Device.Application.Device.Queries.DTO.DeviceDTO>();
            });
        }
    }
    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<ConfigurationSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

                var serviceBusConnection = new ServiceBusConnectionStringBuilder(configuration["EventBusConnection"]);

                return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            });

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            // Add framework services.
            services.AddControllers(options =>
            {

            })
                .AddApplicationPart(typeof(DeviceController).Assembly)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            return services;
        }

        public static IServiceCollection RegisterEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusServiceBus.EventBusServiceBus>(sp =>
            {
                var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusServiceBus.EventBusServiceBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusServiceBus.EventBusServiceBus(serviceBusPersisterConnection, logger,
                    eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
    }

    public class ConfigurationSettings
    {
        public string EventBusConnection { get; set; }
    }
}
