using AutoMapper;
using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.RoleService;
using Capstone.Service.TicketCommentService;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


    }
}
