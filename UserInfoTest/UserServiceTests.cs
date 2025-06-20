using Moq;
using UserInformationApp.Contracts;
using UserInformationApp.Domain;
using UserInformationApp.Service;

namespace UserInfoTest
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IApiClient> _mockApiClient = null!;
        private UserService _userService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockApiClient = new Mock<IApiClient>();
            _userService = new UserService(_mockApiClient.Object);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ReturnsUsers()
        {
            // Arrange
            var page = 1;
            var users = new List<User>
            {
                new User { Id = 1, Email = "a@b.com", First_Name = "A", Last_Name = "B" },
                new User { Id = 2, Email = "c@d.com", First_Name = "C", Last_Name = "D" }
            };
            _mockApiClient
                .Setup(c => c.GetAllUsersAsync(page))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync(page);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("a@b.com", result.First().Email);
            _mockApiClient.Verify(c => c.GetAllUsersAsync(page), Times.Once);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ReturnsEmptyList_WhenNoUsers()
        {
            // Arrange
            var page = 2;
            _mockApiClient
                .Setup(c => c.GetAllUsersAsync(page))
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.GetAllUsersAsync(page);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockApiClient.Verify(c => c.GetAllUsersAsync(page), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsUser_WhenFound()
        {
            // Arrange
            var userId = 5;
            var user = new User { Id = userId, Email = "x@y.com", First_Name = "X", Last_Name = "Y" };
            _mockApiClient
                .Setup(c => c.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result!.Id);
            Assert.AreEqual("x@y.com", result.Email);
            _mockApiClient.Verify(c => c.GetUserByIdAsync(userId), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var userId = 99;
            _mockApiClient
                .Setup(c => c.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.IsNull(result);
            _mockApiClient.Verify(c => c.GetUserByIdAsync(userId), Times.Once);
        }
    }
}
