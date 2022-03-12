using System;
using System.Collections.Generic;
using Eventnet.DataAccess;

namespace Eventnet.Api.IntegrationTests.Helpers;

public class Utilities
{
    public static void InitializeDbForTests(ApplicationDbContext db)
    {
        db.Events.AddRange(GetSeedingMessages());
        db.SaveChanges();
    }

    public static void ReinitializeDbForTests(ApplicationDbContext db)
    {
        db.Events.RemoveRange(db.Events);
        InitializeDbForTests(db);
    }

    private static IEnumerable<EventEntity> GetSeedingMessages() =>
        new List<EventEntity>
        {
            new(new Guid("0C1E0D04-C9D5-43DC-A10A-41CB7BC3DA3A"),
                "user",
                new DateTime(2022, 02, 24),
                null,
                "Event",
                "No description",
                new LocationEntity(49d, 32d))
        };
}