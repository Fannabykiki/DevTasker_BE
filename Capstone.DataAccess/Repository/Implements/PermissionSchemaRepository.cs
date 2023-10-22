using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
    public class PermissionSchemaRepository : BaseRepository<SchemaPermission>, IPermissionSchemaRepository
    {
        public PermissionSchemaRepository(CapstoneContext context) : base(context)
        {
        }
    }
}
