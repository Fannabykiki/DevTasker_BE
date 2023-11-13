using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.ProjectService;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace NUnitTest.DevTasker.Service
{
    public class ProjectServiceTest
    {
        private readonly CapstoneContext _context;
        private ProjectService _projectService;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IBoardRepository> _boardRepositoryMock;
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IProjectMemberRepository> _projectMemberRepositoryMock;
        private Mock<IDatabaseTransaction> _databaseTransactionMock;
        private Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly IMapper _mapper;
        private Mock<ISchemaRepository> _schemaRepository;
        private Mock<IStatusRepository> _statusRepository;

        private Mock<IInterationRepository> _interationRepositoryMock;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IPermissionSchemaRepository> _permissionScemaRepo;
        private Mock<IBoardStatusRepository> _boardStatusRepository;
        private Mock<ITaskRepository> _ticketRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<ITaskTypeRepository> _ticketTypeRepository;
        private Mock<IPriorityRepository> _priorityRepository;

        [SetUp]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _interationRepositoryMock = new Mock<IInterationRepository>();
            _statusRepository = new Mock<IStatusRepository>();
            _databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _schemaRepository = new Mock<ISchemaRepository>();
            _permissionScemaRepo = new Mock<IPermissionSchemaRepository>();
            _boardStatusRepository = new Mock<IBoardStatusRepository>();
            _ticketRepository = new Mock<ITaskRepository>();
            _userRepository = new Mock<IUserRepository>();
            _ticketTypeRepository = new Mock<ITaskTypeRepository>();
            _priorityRepository = new Mock<IPriorityRepository>();


            _projectRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_databaseTransactionMock.Object);

            _projectService = new ProjectService(
                _context,
                _projectRepositoryMock.Object,
                _roleRepositoryMock.Object,
               _mapper,
                _schemaRepository.Object,
               _projectMemberRepositoryMock.Object,
               _boardRepositoryMock.Object,
                _permissionRepositoryMock.Object,
                _interationRepositoryMock.Object,
                _permissionScemaRepo.Object,
                _statusRepository.Object,
                _boardStatusRepository.Object,
                _userRepository.Object,
                _ticketTypeRepository.Object,
                 _priorityRepository.Object,
                _ticketRepository.Object
            );



        }

        [Test]
        public async Task TestCreateProject_Success()
        {
          
        }

        [Test]
        public async Task TestCreateProject_Failure()
        {
        }

        [Test]
        public async Task TestCreateProject_Fail_MissingProjectName()
        {
            
        }

        [Test]
        public async Task TestCreateProject_Fail_InvalidDate()
        {
            
        }

        [Test]
        public async Task TestUpdateProjectInfo_Success()
        {
            
            
        }

        [Test]
        public async Task TestUpdateProjectInfo_Failure()
        {
        }

        [Test]
        public async Task UpdateProjectInfo_Fail_WithEmptyProjectName()
        {
            
        }

        [Test]
        public async Task TestDeleteProject_Success()
        {
           
        }

        [Test]
        public async Task TestDeleteProject_Failure_ProjectNotFound()
        {
           
        }
    }
}




