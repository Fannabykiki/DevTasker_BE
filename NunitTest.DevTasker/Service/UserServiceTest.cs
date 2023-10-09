using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.UserService;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

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
            var result = await _userService.CreateAsync(createUserRequest);

            // Assert
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Once);
            if (result != null)
            {
                Console.WriteLine(" User creation was successful.");
            }
            else
            {
                Console.WriteLine(" User creation was unsuccessful.");
            }

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSucced);
        }

        [Test]
        public async Task CreateAsync_EmailExists()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest
            {
                Email = "cuongbui011@gmail.com",
                Password = "123"
            };

            // Giả lập email đã tồn tại trong cơ sở dữ liệu
            _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(new User { Email = "cuongbui0110.com" }); // Trả về một người dùng có cùng email

            // Act
            var result = await _userService.CreateAsync(createUserRequest);

            // Assert
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Never);

            if (result != null)
            {
                Console.WriteLine(" User creation was successful.");
            }
            else
            {
                Console.WriteLine(" User creation was unsuccessful - Email Exist");
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
                Password = null // Không nhập mật khẩu
            };

            // Act
            var result = await _userService.CreateAsync(createUserRequest);

            // Assert
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Never);

            if (result != null)
            {
                Console.WriteLine("User registration was successful.");
            }
            else
            {
                Console.WriteLine("User registration was unsuccessful. - Password is empty");
            }

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSucced);
        }
        [Test]
        public async Task CreateAsync_MissingEmail()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest
            {
                Email = null, // Không nhập email
                Password = "password123"
            };

            // Act
            var result = await _userService.CreateAsync(createUserRequest);

            // Assert
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChanges(), Times.Never);

            if (result != null)
            {
                Console.WriteLine("User registration was successful.");
            }
            else
            {
                Console.WriteLine(" User registration was unsuccessful. - Email is empty");
            }

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSucced);
        }


    }
}

