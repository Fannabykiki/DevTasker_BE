using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.DataAccess.Repository.Implements
{
    public class SchemaRepository : BaseRepository<Schema>, ISchemaRepository
    {
        public SchemaRepository(CapstoneContext context) : base(context)
        {
        }
    }
}
