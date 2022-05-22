using System;
using System.Linq;
using Eventnet.Api.IntegrationTests.Mocks;
using Eventnet.Api.Services.SaveServices;
using Eventnet.Api.UnitTests.Helpers;
using Eventnet.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Eventnet.Api.IntegrationTests;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
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

            var consumer = services.SingleOrDefault(d => d.ServiceType == typeof(IConsumeEventService));
            services.Remove(consumer!);
            services.AddSingleton<IConsumeEventService, ConsumeEventMock>();

            var publisher = services.SingleOrDefault(d => d.ServiceType == typeof(IPublishEventSaveService));
            services.Remove(publisher!);
            services.AddSingleton<IPublishEventSaveService, PublishEventSaveMock>();

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