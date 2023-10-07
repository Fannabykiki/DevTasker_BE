using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Capstone.DataAccess
{
    public class CapstoneContextFactory : IDesignTimeDbContextFactory<CapstoneContext>
    {
        public CapstoneContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CapstoneContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-OF0V18H\\FANNABY;uid=sa;Password=12345;Database=Capstone;TrustServerCertificate=True;Encrypt=True");
            return new CapstoneContext(optionsBuilder.Options);
        }
    }
}
