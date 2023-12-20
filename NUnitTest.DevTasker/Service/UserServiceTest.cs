using System.Linq.Expressions;
using Capstone.Common.DTOs.User;
using Capstone.Common.Enums;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.UserService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace NUnitTest.DevTasker.Service
{
    public class UserServiceTest
    {
        private readonly CapstoneContext _context;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IProjectMemberRepository> _projectMemberRepositoryMock;
        private Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private UserService _userService;
        private ClaimsIdentity _identity;
        private readonly IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "user@example.com"),  
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            });

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(_identity);
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var contextOptions = new DbContextOptionsBuilder<CapstoneContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new CapstoneContext(contextOptions);
          

            _userService = new UserService(context, _userRepositoryMock.Object, _mapper, _serviceScopeFactoryMock.Object, _httpContextAccessorMock.Object, _projectMemberRepositoryMock.Object);
        }

        // Test Register
        [Test]
        public async Task CreateAsync_Success()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest
            {
                Email = "test@example.com",
                Password = "password123"
            };

            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(new User { UserId = Guid.NewGuid() });

            // Act
            var result = await _userService.Register(createUserRequest);

            // Assert
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Once);
            if (result != null)
            {
                Console.WriteLine("CreateAsync_Success: User creation was successful.");
            }
            else
            {
                Console.WriteLine("CreateAsync_Success: User creation was unsuccessful.");
            }

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSucceed);
        }
        
        [Test]
        public async Task CreateAsync_MissingPassword()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest
            {
                Email = "test@example.com",
                // Thiếu mật khẩu
            };

            // Act
            var result = await _userService.Register(createUserRequest);

            // Assert
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Never);

            if (result != null)
            {
                Console.WriteLine("CreateAsync_MissingPassword: User registration was successful.");
            }
            else
            {
                Console.WriteLine("CreateAsync_MissingPassword: User registration was unsuccessful.");
            }
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSucceed);
        }
        //update profile 
        [Test]
        public async Task UpdateProfileAsync_UserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateProfileRequest = new UpdateProfileRequest
            {
                UserName = "NewUserName",
                Address = "NewAddress",
                Gender = GenderEnum.Male,
                //Avatar = "NewAvatar"
            };
            _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateProfileAsync(updateProfileRequest, userId);

            // Assert
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Never);

            if (result.IsSucceed)
            {
                Console.WriteLine("Error: An error occurred while updating the profile.");
            }
            else
            {
                Console.WriteLine("Error: An error occurred while updating the profile.");
            }

            Assert.IsNotNull(result);
        }
        [Test]
        public async Task UpdateProfileAsync_UserNotFound_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateProfileRequest = new UpdateProfileRequest
            {
                Fullname = "John Doe",
                UserName = "johndoe",
                PhoneNumber = "123456789",
                Address = "123 Main St",
                DoB = new DateTime(1990, 1, 1),
                Gender = GenderEnum.Male
            };

            _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateProfileAsync(updateProfileRequest, userId);

            // Assert
            Assert.IsTrue(result.IsSucceed);
            Assert.IsNull(result.VerifyToken);
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Never);
        }

        // Test verify Account
        [Test]
        public async Task VerifyUser_Success()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = email
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user);

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);

            // Act
            var result = await _userService.VerifyUser(email);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once);
            databaseTransactionMock.Verify(dt => dt.Commit(), Times.Once); 
            databaseTransactionMock.Verify(dt => dt.RollBack(), Times.Never); 

            if (result.IsSucceed)
            {
                Console.WriteLine("VerifyUser_Success: Verification was successful.");
            }
            else
            {
                Console.WriteLine("VerifyUser_Success: Verification was unsuccessful.");
            }
            Assert.IsTrue(result.IsSucceed);
        }
        [Test]
        public async Task TestLoginUserAsync_UsernameNotFound()
        {
            // Arrange
            var login = new LoginRequest
            {
                Email = "nonexistentuser@example.com",
                Password = "testpassword"
            };



            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                 .ReturnsAsync((User)null);

            // Act
            //var result = await _userService.LoginUser(login);

            // Assert
            // Assert.Null(result);
        }

        [Test]
        public async Task TestLoginUserAsync_NullUsername()
        {
            /*// Arrange
            var email = "nonexistentuser@example.com";
            var password = "testpassword";

            // Mock the user repository to return null for user not found
            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny < Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.LoginUser(email, password);

            // Assert
            Assert.Null(result);*/
        }

        [Test]
        public async Task TestLoginUserAsync_NullPassword()
        {
            /*// Arrange
            string username = "testuser";
            string password = null;
            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null);
            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            Assert.Null(result);*/
        }

        [Test]
        public async Task TestLoginUserAsync_EmptyUsername()
        {
            /* // Arrange
             string username = "";
             string password = "testpassword";
             _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                 .ReturnsAsync((User)null);
             // Act
             var result = await _userService.LoginUser(username, password);

             // Assert
             Assert.Null(result);*/
        }

        [Test]
        public async Task TestLoginUserAsync_EmptyPassword()
        {
            /* // Arrange
             string username = "testuser";
             string password = "";
             _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                  .ReturnsAsync((User)null);

             // Act
             var result = await _userService.LoginUser(username, password);

             // Assert
             Assert.Null(result);*/
        }
        [Test]
        public async Task VerifyUser_UserNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null);

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);

            // Act
            var result = await _userService.VerifyUser(email);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
            

            if (result.IsSucceed)
            {
                Console.WriteLine("VerifyUser_UserNotFound: Verification was successful.");
            }
            else
            {
                Console.WriteLine("VerifyUser_UserNotFound: Verification was unsuccessful.");
            }
            Assert.IsFalse(result.IsSucceed);
        }
        // Test forgot Password
        [Test]
        public async Task ForgotPassword_UserNotFound()
        {
            // Arrange
            string email = "nonexistent@example.com";

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null); // Giả lập không tìm thấy người dùng

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);
            // Act
            var result = await _userService.ForgotPassword(email);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
            

            Assert.IsFalse(result);
            Console.WriteLine("UserNotFound: Password reset request failed.");
        }

        [Test]
        public async Task ForgotPassword_EmptyEmail()
        {
            // Arrange
            string email = ""; // Email bị bỏ trống

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);
            // Act
            var result = await _userService.ForgotPassword(email);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
           
            Assert.IsFalse(result);
            Console.WriteLine("EmptyEmail: Password reset request failed.");
        }

        [Test]
        public async Task ForgotPassword_EmailInvalid()
        {
            // Arrange
            string email = "invalid"; 

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);
            // Act
            var result = await _userService.ForgotPassword(email);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);

            Assert.IsFalse(result);
            Console.WriteLine("EmptyEmail: Password reset request failed.");
        }
        [Test]
        public async Task ForgotPassword_Success()
        {
            // Arrange
            string email = "test@example.com";

            var user = new User
            {
                Email = email,
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user); // Giả lập tìm thấy người dùng
            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);

            // Act
            var result = await _userService.ForgotPassword(email);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once);
            databaseTransactionMock.Verify(dt => dt.Commit(), Times.Once);
            databaseTransactionMock.Verify(dt => dt.RollBack(), Times.Never);

            Assert.IsTrue(result);
            Console.WriteLine("Success: Password reset request succeeded.");
        }

        // Test reset Password
        [Test]
        public async Task ResetPasswordSuccess()
        {
            // Arrange
            var resetPasswordRequest = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Password = "newpassword123"
            };

            var user = new User
            {
                Email = resetPasswordRequest.Email,
               
                PassResetToken = "validtoken"
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user); 

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);

            // Act
            var result = await _userService.ResetPassWord(resetPasswordRequest);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once);
            databaseTransactionMock.Verify(dt => dt.Commit(), Times.Once);
            databaseTransactionMock.Verify(dt => dt.RollBack(), Times.Never);

            Assert.IsTrue(result);
            Console.WriteLine("ResetPasswordValidToken: Password reset succeeded with a valid token.");
        }
        [Test]
        public async Task ResetPasswordFail()
        {
            // Arrange
            var resetPasswordRequest = new ResetPasswordRequest
            {
                Email = "test@example.com",
                Password = "newpassword123"
            };

           
            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null);

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);

            // Act
            var result = await _userService.ResetPassWord(resetPasswordRequest);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
           
            Assert.IsFalse(result);
            Console.WriteLine("ResetPasswordInValidToken: Password reset failed.");
        }

        [Test]
        public async Task ChangePassword_Success()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordRequest
            {
                Email = "test@example.com",
                CurrentPassword = "oldPassword123",
                NewPassword = "newPassword123",
                ConfirmPassword = "newPassword123"
            };

            var existingUser = new User
            {
                Email = changePasswordRequest.Email,
            };
            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(existingUser);

            // Password khớp
            byte[] passwordHash;
            byte[] passwordSalt;
            CreatePasswordHash(_userService, changePasswordRequest.NewPassword, out passwordHash, out passwordSalt);
            existingUser.PasswordHash = passwordHash;
            existingUser.PasswordSalt = passwordSalt;

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);

            // Act
            var result = await _userService.ChangePassWord(changePasswordRequest);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once);
            databaseTransactionMock.Verify(dt => dt.Commit(), Times.Once);

            Assert.IsTrue(result);
            Console.WriteLine("Password change succeeded.");
        }

        private void CreatePasswordHash(UserService userService, string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        [Test]
        public async Task ChangePassword_Fail_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var changePasswordRequest = new ChangePasswordRequest
            {
                Email = "test@example.com",
                CurrentPassword = "oldPassword123",
                NewPassword = "newPassword123",
                ConfirmPassword = "differentPassword" 
            };

            var existingUser = new User
            {
                Email = changePasswordRequest.Email,
            };

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);

            // Act
            var result = await _userService.ChangePassWord(changePasswordRequest);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never); 
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never); 

           

            Assert.IsFalse(result);
            Console.WriteLine(" Password change failed when passwords do not match.");
        }


    }
}