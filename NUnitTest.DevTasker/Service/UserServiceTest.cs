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

namespace DevTasker.UnitTest.Service
{

    public class UserServiceTest
    {

        private Mock<IUserRepository> _userRepositoryMock;
        private UserService _userService;



        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(null, _userRepositoryMock.Object);


        }

        //Login Test
        [Test]
        public async Task TestLoginUserAsync()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";

            byte[] passwordSalt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(passwordSalt);
            }

            byte[] passwordHash;
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
         .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            if (result != null)
            {
                Console.WriteLine("Success: Login was successful.");
            }
            else
            {
                Console.WriteLine("Wrong: Login was unsuccessful.");
            }
            Assert.NotNull(result);
            Assert.AreEqual(username, result.UserName);
        }
        [Test]
        public async Task TestLoginUserAsync_InvalidCredentials()
        {
            // Arrange
            var username = "testuser";
            var correctPassword = "testpassword";
            var incorrectPassword = "wrongpassword";

            byte[] passwordSalt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(passwordSalt);
            }

            byte[] passwordHash;
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(correctPassword));
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginUser(username, incorrectPassword);

            // Assert
            if (result != null)
            {
                Console.WriteLine("Success: Login was successful.");
            }
            else
            {
                Console.WriteLine("Wrong: Login was unsuccessful.");
            }
            Assert.Null(result);
        }

        [Test]
        public async Task TestLoginUserAsync_UsernameNotFound()
        {
            // Arrange
            var username = "nonexistentuser";
            var password = "testpassword";

            // Giả lập không có người dùng cùng tên tồn tại
            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            if (result != null)
            {
                Console.WriteLine("Success: Login was successful.");
            }
            else
            {
                Console.WriteLine("Wrong: Username not found.");
            }
            Assert.Null(result);
        }
        [Test]
        public async Task TestLoginUserAsync_NullUsername()
        {
            // Arrange
            string username = null;
            string password = "testpassword";

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task TestLoginUserAsync_NullPassword()
        {
            // Arrange
            string username = "testuser";
            string password = null;

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task TestLoginUserAsync_EmptyUsername()
        {
            // Arrange
            string username = "";
            string password = "testpassword";

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task TestLoginUserAsync_EmptyPassword()
        {
            // Arrange
            string username = "testuser";
            string password = "";

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            Assert.Null(result);
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
        public async Task CreateAsync_MissingEmail()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest
            {
                // Thiếu Email
                Password = "password123"
            };

            // Act
            var result = await _userService.Register(createUserRequest);

            // Assert
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Never);

            if (result != null)
            {
                Console.WriteLine("CreateAsync_MissingEmail: User creation was successful.");
            }
            else
            {
                Console.WriteLine("CreateAsync_MissingEmail: User creation was unsuccessful.");
            }

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSucced);

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
        public async Task CreateAsync_EmailExists()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Giả lập email đã tồn tại trong cơ sở dữ liệu
            _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(new User { Email = "test@example.com" });

            // Act
            var result = await _userService.Register(createUserRequest);

            // Assert
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Never);

            if (result != null)
            {
                Console.WriteLine("CreateAsync_EmailExists: User creation was successful.");
            }
            else
            {
                Console.WriteLine("CreateAsync_EmailExists: User creation was unsuccessful.");
            }
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSucced);

        }


        // Test update profile
        [Test]
        public async Task UpdateProfileAsync_Success()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateProfileRequest = new UpdateProfileRequest
            {
                UserName = "NewUserName",
                Address = "NewAddress",
                Gender = GenderEnum.Male,
                // Avatar = "NewAvatar"
            };

            var user = new User
            {
                UserId = userId,
                // Các thông tin khác của user
            };

            // Giả lập UserRepository và DatabaseTransaction
            _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user);

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(x => x.DatabaseTransaction()).Returns(databaseTransactionMock.Object);

            // Act
            var result = await _userService.UpdateProfileAsync(updateProfileRequest, userId);

            // Assert
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Once);
            databaseTransactionMock.Verify(x => x.Commit(), Times.Once);
            if (result.IsSucced)
            {
                Console.WriteLine("Success: Profile updated successfully.");
            }
            else
            {
                Console.WriteLine("Error: An error occurred while updating the profile.");
            }


            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSucced);
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

            // Giả lập UserRepository trả về null, tức là người dùng không tồn tại
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
            databaseTransactionMock.Verify(dt => dt.Commit(), Times.Once); // Kiểm tra giao dịch đã được commit
            databaseTransactionMock.Verify(dt => dt.RollBack(), Times.Never); // Kiểm tra giao dịch không bị rollback

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
            databaseTransactionMock.Verify(dt => dt.Commit(), Times.Never); // Kiểm tra giao dịch không được commit
            databaseTransactionMock.Verify(dt => dt.RollBack(), Times.Never); // Kiểm tra giao dịch không bị rollback

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
                .ReturnsAsync((User)null); // Giả lập không tìm thấy người dùng

            var databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _userRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(databaseTransactionMock.Object);
            // Act
            var result = await _userService.ForgotPassword(email);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
            databaseTransactionMock.Verify(dt => dt.Commit(), Times.Never);
            databaseTransactionMock.Verify(dt => dt.RollBack(), Times.Never);

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
            databaseTransactionMock.Verify(dt => dt.Commit(), Times.Never);
            databaseTransactionMock.Verify(dt => dt.RollBack(), Times.Never);

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
                // Các thông tin khác của người dùng
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
                // Các thông tin khác của người dùng
                PassResetToken = "validtoken" // Giả lập token hợp lệ
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
            databaseTransactionMock.Verify(dt => dt.Commit(), Times.Never);
            databaseTransactionMock.Verify(dt => dt.RollBack(), Times.Never);

            Assert.IsFalse(result);
            Console.WriteLine("ResetPasswordInValidToken: Password reset failed.");
        }

    }
}