﻿namespace Sample.Hybrid.ConsoleApp;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Hybrid.ConsoleApp.Application;
using Sample.Hybrid.ConsoleApp.EmailService;
using Sample.Hybrid.ConsoleApp.EmailService.Contract;
using SecretStore;
using SlimMessageBus.Host.AzureServiceBus;
using SlimMessageBus.Host.Hybrid;
using SlimMessageBus.Host.Memory;
using SlimMessageBus.Host.Serialization.Json;
using SlimMessageBus.Host.MsDependencyInjection;

public class Startup
{
    public IConfigurationRoot Configuration { get; }

    public Startup(IConfigurationRoot configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(cfg =>
        {
            cfg.AddConfiguration(Configuration);
            cfg.AddConsole();
        });

        services.AddSingleton<MainApplication>();

        services.AddSlimMessageBus((mbb, svp) =>
        {
            // In summary:
            // - The CustomerChangedEvent messages will be going through the SMB Memory provider.
            // - The SendEmailCommand messages will be going through the SMB Azure Service Bus provider.
            // - Each of the bus providers will serialize messages using JSON and use the same DI to resolve consumers/handlers.
            mbb
                // Bus 1
                .AddChildBus("Memory", (mbbChild) =>
                {
                    mbbChild
                        .WithProviderMemory()
                        .AutoDeclareFrom(typeof(CustomerChangedEventHandler).Assembly, consumerTypeFilter: consumerType => consumerType.Namespace.Contains("Application"));
                })
                // Bus 2
                .AddChildBus("AzureSB", (mbbChild) =>
                {
                    var serviceBusConnectionString = Secrets.Service.PopulateSecrets(Configuration["Azure:ServiceBus"]);
                    mbbChild
                        .WithProviderServiceBus(new ServiceBusMessageBusSettings(serviceBusConnectionString))
                        .Produce<SendEmailCommand>(x => x.DefaultQueue("test-ping-queue"))
                        .Consume<SendEmailCommand>(x => x.Queue("test-ping-queue").WithConsumer<SmtpEmailService>());
                })
                .WithSerializer(new JsonMessageSerializer()) // serialization setup will be shared between bus 1 and 2
                .WithProviderHybrid();
        },
        addConsumersFromAssembly: new[] { typeof(CustomerChangedEventHandler).Assembly });
    }
}