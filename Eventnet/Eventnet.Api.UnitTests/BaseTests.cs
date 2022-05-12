using System;
using AutoMapper;
using Eventnet.Api.Config;
using Eventnet.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Api.UnitTests;

public class BaseTests<T>
{
    private readonly DbContextOptions<ApplicationDbContext> contextOptions =
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(nameof(T) + "-db")
            .Options;

    protected IMapper CreateMapper()
    {
        var config = new MapperConfiguration(opt => opt.AddProfile<ApplicationMappingProfile>());
        return config.CreateMapper();
    }

    protected void ApplyToDb(Action<ApplicationDbContext> action)
    {
        using var context = CreateDbContext();
        action(context);
    }
    
    protected TType ApplyToDb<TType>(Func<ApplicationDbContext, TType> action)
    {
        using var context = CreateDbContext();
        return action(context);
    }

    protected ApplicationDbContext CreateDbContext() => new(contextOptions);
}