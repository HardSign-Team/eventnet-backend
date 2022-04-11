using Eventnet.DataAccess;

namespace Eventnet.Api.IntegrationTests.Helpers;

public class Utilities
{
    public static void ReinitializeDbForTests(ApplicationDbContext db)
    {
        db.Events.RemoveRange(db.Events);
        db.Tags.RemoveRange(db.Tags);
        db.SaveChanges();
    }
}