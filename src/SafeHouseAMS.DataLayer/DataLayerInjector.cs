﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SafeHouseAMS.BizLayer.AssistanceRequests;
using SafeHouseAMS.BizLayer.ExploitationEpisodes;
using SafeHouseAMS.BizLayer.LifeSituations;
using SafeHouseAMS.BizLayer.Survivors;
using SafeHouseAMS.DataLayer.Repositories;

namespace SafeHouseAMS.DataLayer
{
    /// <summary>
    /// Методы внедрения логики доступа к данным
    /// </summary>
    public static class DataLayerInjector
    {
        /// <summary>
        /// Добавить сервисы слоя доступа к данным
        /// </summary>
        /// <param name="services">Коллекция сервисов, в которую внедряются службы слоя доступа к данным</param>
        /// <param name="configuration">Конфигурация</param>
        /// <returns>Та же коллекция, для чейнинга</returns>
        public static IServiceCollection ConnectToDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            var inmemoryConncetionString = configuration.GetConnectionString("inMemory");

            if (!string.IsNullOrWhiteSpace(inmemoryConncetionString))
            {
                services.AddDbContext<DataContext>(opt =>
                    opt
                        .UseLazyLoadingProxies()
                        .EnableSensitiveDataLogging()
                        .UseInMemoryDatabase(inmemoryConncetionString),
                ServiceLifetime.Singleton,
                ServiceLifetime.Singleton);

                services.AddScoped<IDatabaseMigrator, DataContext>(_ =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.EnableSensitiveDataLogging().UseInMemoryDatabase(inmemoryConncetionString);
                    return new DataContext(optionsBuilder.Options);
                });
            }

            var postgresConnectionString = configuration.GetConnectionString("postgres");
            if (!string.IsNullOrWhiteSpace(postgresConnectionString))
            {
                services.AddDbContextPool<DataContext>(opt => opt.UseLazyLoadingProxies().UseNpgsql(postgresConnectionString));
                services.AddScoped<IDatabaseMigrator, DataContext>(_ =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseNpgsql(postgresConnectionString);
                    return new DataContext(optionsBuilder.Options);
                });
            }

            services.AddAutoMapper(typeof(DataLayerInjector).Assembly);

            services.TryAddScoped<ISurvivorRepository, SurvivorsRepository>();
            services.TryAddScoped<ILifeSituationDocumentsRepository, LifeSituationDocumentsRepository>();
            services.TryAddScoped<IEpisodesRepository, EpisodesRepository>();
            services.TryAddScoped<IAssistanceRequestsRepository, AssistanceRequestsRepository>();
            return services;
        }
    }
}
