using System.Data.Entity;

namespace BookInfo.DataAccessLayer.EntityFramework
{
    public class MyInitializer :CreateDatabaseIfNotExists<DatabaseContext>
    {
    }
}
