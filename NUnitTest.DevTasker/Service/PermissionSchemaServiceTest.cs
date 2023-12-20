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
using Task = System.Threading.Tasks.Task;

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
        private Mock<IProjectRepository> _projectRepository;
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
            _projectRepository = new Mock<IProjectRepository>();

            _permissionSchemaRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_databaseTransactionMock.Object);
            
                _permissionSchemaService = new PermissionSchemaService(
                null,
                _permissionSchemaRepositoryMock.Object,
                _schemaRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _permissionRepositoryMock.Object,
                _mapperMock.Object,
                _projectRepository.Object
                );
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
        public async Task CreateNewPermissionSchema_Successwithemtydes()
        {
            // Arrange
            var request = new CreateNewSchemaRequest
            {
                SchemaName = "TestSchema",
                Description = ""
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
        public async Task CreateNewPermissionSchema_fail()
        {
            // Arrange
            var request = new CreateNewSchemaRequest
            {
                SchemaName = "",
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
        [Test]
        public async Task UpdateSchema_Fail()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var request = new UpdateSchemaRequest
            {
                SchemaName = "",
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
            Assert.True(result);

        }
        [Test]
        public async Task UpdateSchema_successwwithemptydes()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var request = new UpdateSchemaRequest
            {
                SchemaName = "",
                Description = "UpdatedDescription"
            };

            var schema = new Schema
            {
                SchemaId = schemaId,
                SchemaName = "InitialSchemaName",
                Description = ""
            };

            _schemaRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schema, bool>>>(), null)).ReturnsAsync(schema);

            // Act
            var result = await _permissionSchemaService.UpdateSchema(schemaId, request);

            // Assert
            Assert.True(result);

        }
        
        [Test]
        public async Task GetPermissionSchemaById_ValidSchemaId_ReturnsPermissionSchema()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _schemaRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Schema, bool>>>(), null))
                .ReturnsAsync(new Schema { SchemaId = schemaId });
            _projectRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync(new Project { ProjectId = projectId, SchemasId = schemaId });

            // Act
            var result = await _permissionSchemaService.GetPermissionSchemaById(schemaId, projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(schemaId, result.SchemaId);
        }
        [Test]
        public async Task GrantSchemaPermissionRoles_Success()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var request = new GrantPermissionSchemaRequest
            {
                ProjectId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                PermissionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _projectRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), It.IsAny<Expression<Func<Project, object>>>()))
                .ReturnsAsync(new Project { SchemasId = schemaId });
            _permissionSchemaRepositoryMock.Setup(r => r.GetAllWithOdata(It.IsAny<Expression<Func<SchemaPermission, bool>>>(), null))
                .ReturnsAsync(new List<SchemaPermission>());

            // Act
            var result = await _permissionSchemaService.GrantSchemaPermissionRoles(schemaId, request, Guid.NewGuid());

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task GrantSchemaPermissionRoles_Fail()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var request = new GrantPermissionSchemaRequest
            {
                ProjectId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                PermissionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _projectRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), It.IsAny<Expression<Func<Project, object>>>()))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _permissionSchemaService.GrantSchemaPermissionRoles(schemaId, request, Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task RevokeSchemaPermissionRoles_Success()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var request = new RevokePermissionSchemaRequest
            {
                ProjectId = Guid.NewGuid(),
                RoleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                PermissionId = Guid.NewGuid()
            };

            _projectRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), It.IsAny<Expression<Func<Project, object>>>()))
                .ReturnsAsync(new Project { SchemasId = schemaId });
            _permissionSchemaRepositoryMock.Setup(r => r.GetAllWithOdata(It.IsAny<Expression<Func<SchemaPermission, bool>>>(), null))
                .ReturnsAsync(new List<SchemaPermission>());

            // Act
            var result = await _permissionSchemaService.RevokeSchemaPermissionRoles(schemaId, request, Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task RevokeSchemaPermissionRoles_Fail()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var request = new RevokePermissionSchemaRequest
            {
                ProjectId = Guid.NewGuid(),
                RoleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                PermissionId = Guid.NewGuid()
            };

            _projectRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), It.IsAny<Expression<Func<Project, object>>>()))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _permissionSchemaService.RevokeSchemaPermissionRoles(schemaId, request, Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetSchemaById_InvalidSchemaId_ReturnsNull()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            _schemaRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schema, bool>>>(), null))
                .ReturnsAsync((Schema)null);

            // Act
            var result = await _permissionSchemaService.GetSchemaById(schemaId);

            // Assert
            Assert.IsNull(result);
        }
        [Test]
        public async Task RevokeSchemaPermissionRoles_ValidRequest_ReturnsTrue()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var request = new RevokePermissionSchemaRequest
            {
                ProjectId = Guid.NewGuid(),
                RoleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                PermissionId = Guid.NewGuid()
            };

            _projectRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), It.IsAny<Expression<Func<Project, object>>>()))
                .ReturnsAsync(new Project { SchemasId = schemaId });
            _permissionSchemaRepositoryMock.Setup(r => r.GetAllWithOdata(It.IsAny<Expression<Func<SchemaPermission, bool>>>(), null))
                .ReturnsAsync(new List<SchemaPermission>());

            // Act
            var result = await _permissionSchemaService.RevokeSchemaPermissionRoles(schemaId, request, Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task RevokeSchemaPermissionRoles_InvalidProjectId_ReturnsFalse()
        {
            // Arrange
            var schemaId = Guid.NewGuid();
            var request = new RevokePermissionSchemaRequest
            {
                ProjectId = Guid.NewGuid(),
                RoleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                PermissionId = Guid.NewGuid()
            };

            _projectRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), It.IsAny<Expression<Func<Project, object>>>()))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _permissionSchemaService.RevokeSchemaPermissionRoles(schemaId, request, Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetSchemaByName_InvalidSchemaName_ReturnsNull()
        {
            // Arrange
            var schemaName = "NonExistentSchema";
            _schemaRepositoryMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schema, bool>>>(), null))
                .ReturnsAsync((Schema)null);

            // Act
            var result = await _permissionSchemaService.GetSchemaByName(schemaName);

            // Assert
            Assert.IsNull(result);
        }



    }
}

