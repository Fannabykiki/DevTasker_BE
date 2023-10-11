﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Capstone.DataAccess
{
    public class CapstoneContextFactory : IDesignTimeDbContextFactory<CapstoneContext>
    {
        public CapstoneContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CapstoneContext>();
            optionsBuilder.UseSqlServer("Server=capstone.ct7404ko8aq4.us-east-2.rds.amazonaws.com;uid=devtasker;Password=PhanNam2001;Database=Capstone;TrustServerCertificate=True;Encrypt=True");
            return new CapstoneContext(optionsBuilder.Options);
        }
    }
}
