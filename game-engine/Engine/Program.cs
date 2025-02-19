﻿using System;
using System.Threading.Tasks;
using Engine.Handlers.Actions;
using Engine.Handlers.Actions.Retrieval;
// using Engine.Handlers.Collisions;
using Engine.Handlers.Interfaces;
using Engine.Handlers.Resolvers;
using Engine.Interfaces;
using Engine.Models;
using Engine.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Engine
{
    public class Program
    {
        public static IConfigurationRoot Configuration;

        private static Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            using var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var signalRService = provider.GetRequiredService<ISignalRService>();

            signalRService.Startup();

            return host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureServices(GetServiceConfiguration);

        private static void GetServiceConfiguration(HostBuilderContext _, IServiceCollection services)
        {
            RegisterConfigFiles(services);

            // Singletons are instantiated once and remain the same through the lifecycle of the app.
            // Use these for State services
            RegisterSingletonServices(services);

            // Scoped services are created once for each scope
            RegisterScopedServices(services);

            RegisterActionHandlers(services);

            RegisterResolvers(services);
        }

        private static void RegisterConfigFiles(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);
            Configuration = builder.Build();
            services.Configure<EngineConfig>(Configuration);
        }

        private static void RegisterResolvers(IServiceCollection services)
        {
            services.AddTransient<IActionHandlerResolver, ActionHandlerResolver>();
        }

        private static void RegisterScopedServices(IServiceCollection services)
        {
            services.AddScoped<ITickProcessingService, TickProcessingService>();
            services.AddScoped<IActionService, ActionService>();
        }

        private static void RegisterSingletonServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IWorldStateService, WorldStateService>();
            services.AddSingleton<ISignalRService, SignalRService>();
            services.AddSingleton<IEngineService, EngineService>();
            services.AddSingleton<ICalculationService, CalculationService>();
        }

        private static void RegisterActionHandlers(IServiceCollection services)
        {
            // services.AddTransient<IActionHandler, SendBuilderActionHandler>();
            services.AddTransient<IActionHandler, SendLumberActionHandler>();
            services.AddTransient<IActionHandler, SendMinerActionHandler>();
            services.AddTransient<IActionHandler, SendScoutActionHandler>();
            services.AddTransient<IActionHandler, SendFarmActionHandler> ();
            services.AddTransient<IActionHandler, StartCampfireActionHandler> ();
        }


        public static void CloseApplication()
        {
            Environment.Exit(0);
        }
    }
}