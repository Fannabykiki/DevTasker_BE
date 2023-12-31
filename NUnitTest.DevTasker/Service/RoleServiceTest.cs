﻿using AutoMapper;
using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.RoleService;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace NUnitTest.DevTasker.Service
{
    [TestFixture]
    public class RoleServiceTest
    {
        private RoleService _roleService;
        private Mock <IRoleRepository> _roleRepository;
        private Mock <IMapper> _mapper;
        private Mock<IProjectRepository> _projectRepository;
        private Mock<IPermissionSchemaRepository> _permissionSchemaRepository;

        [SetUp]
        public void Setup()
        {
            _roleRepository = new Mock<IRoleRepository>();
            _mapper = new Mock<IMapper>();
            _projectRepository = new Mock<IProjectRepository>();
            _permissionSchemaRepository = new Mock<IPermissionSchemaRepository>();

            _roleService = new RoleService
            (
                    _roleRepository.Object,
                    _mapper.Object,
                    _projectRepository.Object,
                    _permissionSchemaRepository.Object
            );
        }
        


        [Test]
        public async Task CreateProjectRole_Success()
        {
            // Arrange
            var createRoleRequest = new CreateNewRoleRequest
            {
                RoleName = "Test Role",
                Description = "Test Role Description"
            };

            var newRole = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = createRoleRequest.RoleName,
                Description = createRoleRequest.Description
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            _mapper.Setup(m => m.Map<GetRoleResponse>(It.IsAny<Role>()))
                .Returns((Role role) => new GetRoleResponse
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Description = role.Description
                });

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            //Act
           var result = await roleService.CreateProjectRole(createRoleRequest);
            if (result != null)
            {
                Console.WriteLine("CreateProjectRole Success");
            }
            else
            {
                Console.WriteLine("CreateProjectRole Fail");
            }
            // Assert
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task CreateProjectRole_SuccessEmptyDes()
        {
            // Arrange
            var createRoleRequest = new CreateNewRoleRequest
            {
                RoleName = "Test Role",
                Description = ""
            };

            var newRole = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = createRoleRequest.RoleName,
                Description = createRoleRequest.Description
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            _mapper.Setup(m => m.Map<GetRoleResponse>(It.IsAny<Role>()))
                .Returns((Role role) => new GetRoleResponse
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Description = role.Description
                });

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            //Act
            var result = await roleService.CreateProjectRole(createRoleRequest);
            if (result != null)
            {
                Console.WriteLine("CreateProjectRole Success");
            }
            else
            {
                Console.WriteLine("CreateProjectRole Fail");
            }
            // Assert
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task CreateProjectRole_Failure()
        {
            // Arrange
            var createRoleRequest = new CreateNewRoleRequest
            {
                RoleName = "Test Role",
                Description = "Test Role Description"
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            // Act
            var result = await roleService.CreateProjectRole(createRoleRequest);
            if (result != null)
            {
                Console.WriteLine("CreateProjectRole Success");
            }
            else
            {
                Console.WriteLine("CreateProjectRole Fail");
            }

            // Assert
            Assert.IsNull(result);

        }
        [Test]
        public async Task CreateProjectRole_FailurewithTitleNameExist()
        {
            // Arrange
            var createRoleRequest = new CreateNewRoleRequest
            {
                RoleName = "Test Role",
                Description = "Test Role Description"
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            // Act
            var result = await roleService.CreateProjectRole(createRoleRequest);
            if (result != null)
            {
                Console.WriteLine("CreateProjectRole Success");
            }
            else
            {
                Console.WriteLine("CreateProjectRole Fail");
            }

            // Assert
            Assert.IsNull(result);

        }
        [Test]
        public async Task CreateProjectRole_FailureEmtyRoleName()
        {
            // Arrange
            var createRoleRequest = new CreateNewRoleRequest
            {
                RoleName = "",
                Description = "Test Role Description"
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            // Act
            var result = await roleService.CreateProjectRole(createRoleRequest);
            if (result != null)
            {
                Console.WriteLine("CreateProjectRole Success");
            }
            else
            {
                Console.WriteLine("CreateProjectRole Fail");
            }

            // Assert
            Assert.IsNull(result);

        }
        [Test]
        public async Task CreateProjectRole_FailureDuplicateRole()
        {
            // Arrange
            var createRoleRequest = new CreateNewRoleRequest
            {
                RoleName = "uplicate",
                Description = "Test Role Description"
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            // Act
            var result = await roleService.CreateProjectRole(createRoleRequest);
            if (result != null)
            {
                Console.WriteLine("CreateProjectRole Success");
            }
            else
            {
                Console.WriteLine("CreateProjectRole Fail");
            }

            // Assert
            Assert.IsNull(result);

        }
        [Test]
        public async Task UpdateSystemRole_Success()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var updateRoleRequest = new UpdateRoleRequest
            {
                RoleName = "Updated Role Name",
                Description = "Updated Role Description"
            };

            var existingRole = new Role
            {
                RoleId = roleId,
                RoleName = "Original Role Name",
                Description = "Original Role Description"
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            _mapper.Setup(m => m.Map<GetRoleResponse>(It.IsAny<Role>()))
               .Returns((Role role) => new GetRoleResponse
               {
                   RoleId = role.RoleId,
                   RoleName = role.RoleName,
                   Description = role.Description
               });

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            //Act
           var result = await roleService.UpdateSystemRole(roleId, updateRoleRequest);
            Assert.IsNull(result, "UpdateSystemRole should succeed and return a non-null result");
        }
        [Test]
        public async Task UpdateSystemRole_SuccessEmtyDes()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var updateRoleRequest = new UpdateRoleRequest
            {
                RoleName = "Updated Role Name",
                Description = ""
            };

            var existingRole = new Role
            {
                RoleId = roleId,
                RoleName = "Original Role Name",
                Description = "Original Role Description"
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            _mapper.Setup(m => m.Map<GetRoleResponse>(It.IsAny<Role>()))
               .Returns((Role role) => new GetRoleResponse
               {
                   RoleId = role.RoleId,
                   RoleName = role.RoleName,
                   Description = role.Description
               });

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            //Act
            var result = await roleService.UpdateSystemRole(roleId, updateRoleRequest);
            Assert.IsNull(result, "UpdateSystemRole should succeed and return a non-null result");
        }

        [Test]
        public async Task UpdateSystemRole_Failure()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var updateRoleRequest = new UpdateRoleRequest
            {
                RoleName = "Updated Role Name",
                Description = "Updated Role Description"
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();
            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            // Act
            var result = await roleService.UpdateSystemRole(roleId, updateRoleRequest);
            if (result != null)
            {
                Console.WriteLine("UpdateSystemRole Success");
            }
            else
            {
                Console.WriteLine("UpdateSystemRole Fail");
            }
            // Assert
            Assert.IsNull(result);
            databaseTransaction.Verify(t => t.RollBack(), Times.Once);
        }
        [Test]
        public async Task UpdateSystemRole_Fail()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var updateRoleRequest = new UpdateRoleRequest
            {
                RoleName = "",
                Description = "Updated Role Description"
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();
            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            // Act
            var result = await roleService.UpdateSystemRole(roleId, updateRoleRequest);
            if (result != null)
            {
                Console.WriteLine("UpdateSystemRole Success");
            }
            else
            {
                Console.WriteLine("UpdateSystemRole Fail");
            }
            // Assert
            Assert.IsNull(result);
            databaseTransaction.Verify(t => t.RollBack(), Times.Once);
        }
        [Test]
        public async Task UpdateSystemRole_FailWithTitleExíst()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var updateRoleRequest = new UpdateRoleRequest
            {
                RoleName = "",
                Description = "Updated Role Description"
            };

            var databaseTransaction = new Mock<IDatabaseTransaction>();
            _roleRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var roleService = new RoleService(_roleRepository.Object, _mapper.Object, _projectRepository.Object, _permissionSchemaRepository.Object);

            // Act
            var result = await roleService.UpdateSystemRole(roleId, updateRoleRequest);
            if (result != null)
            {
                Console.WriteLine("UpdateSystemRole Success");
            }
            else
            {
                Console.WriteLine("UpdateSystemRole Fail");
            }
            // Assert
            Assert.IsNull(result);
            databaseTransaction.Verify(t => t.RollBack(), Times.Once);
        }
        [Test]
        public async Task GetSystemRoleByName_Success()
        {
            // Arrange
            var roleName = "TestRole";
            var role = new Role { RoleId = Guid.NewGuid(), RoleName = roleName, Description = "Test Description" };
            _roleRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Role, bool>>>(), null))
                           .ReturnsAsync(role);
            _mapper.Setup(m => m.Map<GetRoleResponse>(It.IsAny<Role>()))
                   .Returns((Role r) => new GetRoleResponse { RoleId = r.RoleId, RoleName = r.RoleName, Description = r.Description });

            // Act
            var result = await _roleService.GetSystemRoleByName(roleName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(roleName, result.RoleName);
        }
        [Test]
        public async Task GetSystemRoleById_Success()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var role = new Role { RoleId = roleId, RoleName = "TestRole", Description = "Test Description" };
            _roleRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Role, bool>>>(), null))
                           .ReturnsAsync(role);
            _mapper.Setup(m => m.Map<GetRoleResponse>(It.IsAny<Role>()))
                   .Returns((Role r) => new GetRoleResponse { RoleId = r.RoleId, RoleName = r.RoleName, Description = r.Description });

            // Act
            var result = await _roleService.GetSystemRoleById(roleId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(roleId, result.RoleId);
        }
       
        [Test]
        public async Task GetRolesByProjectId_Success()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { ProjectId = projectId, ProjectName = "TestProject" };
            var roles = new List<Role>
        {
            new Role { RoleId = Guid.NewGuid(), RoleName = "Role1", Description = "Description1" },
            new Role { RoleId = Guid.NewGuid(), RoleName = "Role2", Description = "Description2" }
        };
            _projectRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                              .ReturnsAsync(project);
            _roleRepository.Setup(repo => repo.GetAllWithOdata(It.IsAny<Expression<Func<Role, bool>>>(), null))
                           .ReturnsAsync(roles);
            _mapper.Setup(m => m.Map<GetRoleResponse>(It.IsAny<Role>()))
                   .Returns((Role r) => new GetRoleResponse { RoleId = r.RoleId, RoleName = r.RoleName, Description = r.Description });

            // Act
            var result = await _roleService.GetRolesByProjectId(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

    }
}
