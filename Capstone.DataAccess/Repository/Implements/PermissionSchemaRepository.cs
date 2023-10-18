﻿using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
    public class PermissionSchemaRepository : BaseRepository<Schema>, IPermissionSchemaRepository
    {
        public PermissionSchemaRepository(CapstoneContext context) : base(context)
        {
        }
    }
}
