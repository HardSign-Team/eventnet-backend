using System;
using System.Linq;
using Eventnet.Api.Config;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.IntegrationTests.Mocks;
using Eventnet.DataAccess;
using Eventnet.Infrastructure.PhotoServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Eventnet.Api.IntegrationTests;

public class RabbitMqTestFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            services.Remove(descriptor!);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var rabbitConfig = services.SingleOrDefault(d => d.ServiceType == typeof(RabbitMqConfig));
            services.Remove(rabbitConfig!);
            var testRabbitMqConfig = new RabbitMqConfig
            {
                HostName = "localhost", 
                Queue = "MyTestQueue", 
                Port = 5672,
                RecommendedMessageSizeInBytes = 128 * 1024 * 1024
            };
            services.AddSingleton(testRabbitMqConfig);

            var photoToStorageSaveService = services.SingleOrDefault(
                d => d.ServiceType == typeof(IPhotoToStorageSaveService));
            services.Remove(photoToStorageSaveService!);
            services.AddSingleton<IPhotoToStorageSaveService, PhotoToStorageSaveServiceMock>();

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            var logger = scopedServices
                .GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();
            db.Database.EnsureCreated();
            try
            {
                Utilities.ReinitializeDbForTests(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "An error occurred seeding the " +
                    "database with test messages. Error: {Message}",
                    ex.Message);
            }
        });
    }
}