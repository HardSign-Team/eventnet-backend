using System;
using System.Collections.Generic;
using System.Linq;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.IntegrationTests.Helpers;

public static class ApplicationDbExtensions
{
    private static readonly Random Rnd = new();

    public static void Clear(this ApplicationDbContext db)
    {
        Utilities.ReinitializeDbForTests(db);
    }

    public static void AddUsers(this ApplicationDbContext db)
    {
        var users = new[]
        {
            new UserEntity
            {
                UserName = "gODeaLOAple",
                PasswordHash = "jopabonana",
                Email = "gODeaLOAple@gmail.com"
            },
            new UserEntity
            {
                UserName = "lapagovna",
                PasswordHash = "jopabonana",
                Email = "lapagovna@gmail.com"
            },
            new UserEntity
            {
                UserName = "iva",
                PasswordHash = "jopabonana",
                Email = "iva@gmail.com"
            }
        };
        db.Users.AddRange(users);
        db.SaveChanges();
    }

    public static void AddEvents(this ApplicationDbContext db, IEnumerable<UserEntity> users)
    {
        var ids = users.Select(x => x.Id).ToArray();
        var events = new[]
        {
            new EventEntity(Guid.NewGuid(),
                Rnd.Choice(ids),
                DateTime.Now,
                DateTime.Now + TimeSpan.FromDays(10),
                "Event1",
                "No description",
                new LocationEntity(56.840234511156446, 60.616096578611625)),
            new EventEntity(Guid.NewGuid(),
                Rnd.Choice(ids),
                DateTime.Now,
                DateTime.Now + TimeSpan.FromDays(10),
                "Event2",
                "No description",
                new LocationEntity(56.84391149402939, 60.65316741081937)),
            new EventEntity(Guid.NewGuid(),
                Rnd.Choice(ids),
                DateTime.Now,
                DateTime.Now + TimeSpan.FromDays(10),
                "Event3",
                "No description",
                new LocationEntity(56.81781873425029, 60.61238225939552)),
            new EventEntity(Guid.NewGuid(),
                Rnd.Choice(ids),
                DateTime.Now,
                DateTime.Now + TimeSpan.FromDays(10),
                "Event4",
                "No description",
                new LocationEntity(56.81787873103509, 60.54074620394152))
        };
        db.Events.AddRange(events);
        db.SaveChanges();
    }

    public static void AddTags(this ApplicationDbContext db)
    {
        var tags = new[]
        {
            new TagEntity("Tag1"),
            new TagEntity("Tag2"),
            new TagEntity("Tag3"),
            new TagEntity("Tag4"),
            new TagEntity("Tag5"),
            new TagEntity("Tag6")
        };
        db.Tags.AddRange(tags);
        db.SaveChanges();
    }
}