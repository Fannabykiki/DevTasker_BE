using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Capstone.DataAccess
{
    public class CapstoneContextFactory : IDesignTimeDbContextFactory<CapstoneContext>
    {
        public CapstoneContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CapstoneContext>();
            optionsBuilder.UseSqlServer("Server=localhost;uid=sa;Password=123;Database=Capstone;TrustServerCertificate=True;Encrypt=True");
            return new CapstoneContext(optionsBuilder.Options);
        }
    }
}
