using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using urlShortener.DataModel;

namespace urlShortener.DBContext
{
    public class shortnerDBContext : DbContext
    {
        public shortnerDBContext(DbContextOptions<shortnerDBContext> options) : base(options) 
        {
            try
            {
                var dbCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (dbCreator != null)
                {
                    if (!dbCreator.CanConnect()) dbCreator.Create();
                    if (!dbCreator.HasTables()) dbCreator.CreateTables();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public DbSet<shortener> urlShortner {  get; set; }
        public DbSet<users> user { get; set; }
    }
}
