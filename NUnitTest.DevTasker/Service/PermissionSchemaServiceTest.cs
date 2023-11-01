using AutoMapper;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.DTOs.Schema;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Capstone.Service.PermissionSchemaService;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace NUnitTest.DevTasker.Service
{
    public class PermissionSchemaServiceTest
    {
        private PermissionSchemaService _permissionSchemaService;
        private Mock<IPermissionSchemaRepository> _permissionSchemaRepositoryMock;
        private Mock<ISchemaRepository> _schemaRepositoryMock;
        private Mock<IPermissionRepository> _permissionRepositoryMock;
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IDatabaseTransaction> _databaseTransactionMock;
        private Mock<IMapper> _mapperMock;
        private readonly ILoggerManager _logger;

        [SetUp]
        public void Setup()
        {
            _permissionSchemaRepositoryMock = new Mock<IPermissionSchemaRepository>();
            _schemaRepositoryMock = new Mock<ISchemaRepository>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _mapperMock = new Mock<IMapper>();

            _permissionSchemaRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_databaseTransactionMock.Object);
            
                _permissionSchemaService = new PermissionSchemaService(
                null,
                _permissionSchemaRepositoryMock.Object,
                _schemaRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _permissionRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Test]
        public async Task CreateNewPermissionSchema_Success()
        {
            // Arrange
            var request = new CreateNewSchemaRequest
            {
                SchemaName = "TestSchema",
                Description = "Test Description"
            };

            var newSchema = new Schema
            {
                SchemaId = Guid.NewGuid(),
                SchemaName = request.SchemaName,
                Description = request.Description
            };

            _schemaRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Schema>())).ReturnsAsync(newSchema);
            _permissionRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Permission, bool>>>(), null))
                .Returns(new List<Permission> { new Permission { PermissionId = Guid.NewGuid() } }.AsQueryable());

            // Act
            var result = await _permissionSchemaService.CreateNewPermissionSchema(request);

            // Assert
            Assert.IsTrue(result);
          
        }

        [Test]
        public async Task UpdateSchema_Success()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var request = new UpdateSchemaRequest
            {
                SchemaName = "UpdatedSchemaName",
                Description = "UpdatedDescription"
            };

            var schema = new Schema
            {
                SchemaId = schemaId,
                SchemaName = "InitialSchemaName",
                Description = "InitialDescription"
            };

            _schemaRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schema, bool>>>(), null)).ReturnsAsync(schema);

            // Act
            var result = await _permissionSchemaService.UpdateSchema(schemaId, request);

            // Assert
            Assert.IsTrue(result);
            
        }



    }
}

