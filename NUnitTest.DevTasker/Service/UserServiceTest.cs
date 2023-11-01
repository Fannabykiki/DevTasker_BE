using System.Text;
using Capstone.DataAccess.Repository.Interfaces;
using Moq;
using Capstone.Service.UserService;
using NUnit.Framework;
using Capstone.DataAccess.Entities;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Capstone.Common.DTOs.User;
using Capstone.Common.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Capstone.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace NUnitTest.DevTasker.Service
{

    public class UserServiceTest
    {

        private Mock<IUserRepository> _userRepositoryMock;
        private UserService _userService;
        private readonly IMapper _mapper;
        private readonly CapstoneContext _context;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
             _userService = new UserService(_context, _userRepositoryMock.Object, _mapper, _serviceScopeFactory);
            
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
            Assert.IsTrue(result.IsSucced);
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
            Assert.IsFalse(result.IsSucced);
        }


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

            if (result.IsSucced)
            {
                Console.WriteLine("Success: Profile updated successfully.");
            }
            else
            {
                Console.WriteLine("Error: An error occurred while updating the profile.");
            }

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSucced);
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

            if (result.IsSucced)
            {
                Console.WriteLine("VerifyUser_Success: Verification was successful.");
            }
            else
            {
                Console.WriteLine("VerifyUser_Success: Verification was unsuccessful.");
            }
            Assert.IsTrue(result.IsSucced);
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
            

            if (result.IsSucced)
            {
                Console.WriteLine("VerifyUser_UserNotFound: Verification was successful.");
            }
            else
            {
                Console.WriteLine("VerifyUser_UserNotFound: Verification was unsuccessful.");
            }
            Assert.IsFalse(result.IsSucced);
        }


        // Test forgot Password
        [Test]
        public async Task ForgotPassword_UserNotFound()
        {
            // Arrange
            string email = "nonexistent@example.com";

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null); 

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
        public async Task ForgotPassword_Success()
        {
            // Arrange
            string email = "test@example.com";

            var user = new User
            {
                Email = email,
                
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user); 
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
                // Các thông tin khác của người dùng
                PassResetToken = "validtoken"
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user); // Giả lập tìm thấy người dùng

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

            // Giả lập không tìm thấy người dùng
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

           

            Assert.IsFalse(result); // Kiểm tra rằng kết quả là false
            Console.WriteLine(" Password change failed when passwords do not match.");
        }


    }
}