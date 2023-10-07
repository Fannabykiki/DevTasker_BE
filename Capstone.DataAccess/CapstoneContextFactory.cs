using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Capstone.DataAccess
{
    public class CapstoneContextFactory : IDesignTimeDbContextFactory<CapstoneContext>
    {
        public CapstoneContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CapstoneContext>();
            optionsBuilder.UseSqlServer("Server=staid-strider-6632.8nk.cockroachlabs.cloud;uid=devtasker;Password=oGbVtXnx1XpjBikXQul6Zg;Database=Capstone;Trusted_Connection=False;Encrypt=True");
            return new CapstoneContext(optionsBuilder.Options);
        }
    }
}
