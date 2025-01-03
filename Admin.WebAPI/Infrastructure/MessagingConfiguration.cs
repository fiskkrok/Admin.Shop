﻿using Admin.Application.Common.Interfaces;
using Admin.Infrastructure.Services.MessageBus;
using Admin.WebAPI.Messaging.Consumers;

using FluentValidation;

using MassTransit;

namespace Admin.WebAPI.Infrastructure;

public static class MessagingConfiguration
{
    public static IServiceCollection AddMessagingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add SignalR with Redis backplane
        
            var redisConnection = configuration.GetConnectionString("Redis");

            services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = true;
                    options.MaximumReceiveMessageSize = 102400; // 100 KB
                    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(20);
                })
                .AddStackExchangeRedis(redisConnection!, options =>
                {
                    options.Configuration.ChannelPrefix = "Admin_";
                });
        

        // Configure RabbitMQ Settings and Service
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<RabbitMQService>();
        services.AddSingleton<IMessageBusService>(sp => sp.GetRequiredService<RabbitMQService>());
        services.AddHostedService(sp => sp.GetRequiredService<RabbitMQService>());

        // Configure MassTransit
        services.AddMassTransit(x =>
        {
            // Register consumers
            x.AddConsumer<ProductCreatedConsumer>();
            x.AddConsumer<StockUpdatedConsumer>();
            x.AddConsumer<ImageProcessingConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>()
                                       ?? throw new InvalidOperationException("RabbitMQ settings are missing");

                cfg.Host(rabbitMqSettings.Host, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                // Configure error handling and retry policies
                cfg.UseMessageRetry(r =>
                {
                    r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
                    r.Ignore<ValidationException>();
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}