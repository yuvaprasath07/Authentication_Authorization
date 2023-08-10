using Microsoft.EntityFrameworkCore;

namespace Authentication_Authorization.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        public DbSet<UserCreate> UserLogin { get; set; }
        public DbSet<DownloadDataModel> pathDetails { get; set; }
    }
}
