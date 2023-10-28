using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Capstone.DataAccess
{
    public class CapstoneContextFactory : IDesignTimeDbContextFactory<CapstoneContext>
    {
        public CapstoneContext CreateDbContext(string[] args)
        {//
            var optionsBuilder = new DbContextOptionsBuilder<CapstoneContext>();
            optionsBuilder.UseSqlServer("Server=103.184.113.101;uid=sa;Password=YourStrong@Passw;Database=Capstone;TrustServerCertificate=True;Encrypt=True");
            // optionsBuilder.UseSqlServer("Server=capstone.ct7404ko8aq4.us-east-2.rds.amazonaws.com;uid=devtasker;Password=PhanNam2001;Database=Capstone;TrustServerCertificate=True;Encrypt=True");
            //optionsBuilder.UseSqlServer("Server=DESKTOP-OF0V18H\\FANNABY;uid=sa;Password=123456;Database=Capstone;TrustServerCertificate=True;Encrypt=True");
            //"DBConnString": "Server=HI\\SQLEXPRESS;uid=dev;Password=123;Database=Capstone;TrustServerCertificate=True;Encrypt=True"
            return new CapstoneContext(optionsBuilder.Options);
        }
    }
}
