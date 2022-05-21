using Eventnet.DataAccess;

namespace Eventnet.Api.UnitTests.Helpers;

public class Utilities
{
    public static void ReinitializeDbForTests(ApplicationDbContext db)
    {
        ClearDb(db);
    }

    private static void ClearDb(ApplicationDbContext db)
    {
        db.Events.RemoveRange(db.Events);
        db.Tags.RemoveRange(db.Tags);
        db.Subscriptions.RemoveRange(db.Subscriptions);
        db.Users.RemoveRange(db.Users);
        db.Marks.RemoveRange(db.Marks);
        db.SaveChanges();
    }
}