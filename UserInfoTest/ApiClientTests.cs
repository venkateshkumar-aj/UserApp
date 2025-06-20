using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using UserInformationApp.Infrastructure;

namespace UserInfoTest
{
    [TestClass]
    public sealed class ApiClientTests
    {
        private static ApiOptions GetApiOptions() =>
            new ApiOptions { BaseUrl = "https://example.com" };

        private static IOptions<ApiOptions> GetOptions() =>
            Options.Create(GetApiOptions());

        private static HttpClient GetHttpClient(HttpResponseMessage response, out Mock<HttpMessageHandler> handlerMock)
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            return new HttpClient(handlerMock.Object);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ReturnsUsers_WhenResponseIsSuccessful()
        {
            // Arrange
            var users = new List<UserInformationApp.Domain.User>
            {
                new UserInformationApp.Domain.User { Id = 1, Email = "a@b.com", First_Name = "A", Last_Name = "B" }
            };
            var usersResponse = new UsersResponse { Data = users };
            var json = JsonSerializer.Serialize(usersResponse);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            var httpClient = GetHttpClient(response, out _);
            var apiClient = new ApiClient(httpClient, GetOptions());

            // Act
            var result = await apiClient.GetAllUsersAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("a@b.com", result.First().Email);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ReturnsEmpty_WhenResponseHasNoData()
        {
            // Arrange
            var usersResponse = new UsersResponse { Data = null };
            var json = JsonSerializer.Serialize(usersResponse);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            var httpClient = GetHttpClient(response, out _);
            var apiClient = new ApiClient(httpClient, GetOptions());

            // Act
            var result = await apiClient.GetAllUsersAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsUser_WhenResponseIsSuccessful()
        {
            // Arrange
            var user = new UserInformationApp.Domain.User { Id = 2, Email = "c@d.com", First_Name = "C", Last_Name = "D" };
            var userResponse = new UserResponse { Data = user };
            var json = JsonSerializer.Serialize(userResponse);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            var httpClient = GetHttpClient(response, out _);
            var apiClient = new ApiClient(httpClient, GetOptions());

            // Act
            var result = await apiClient.GetUserByIdAsync(2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual("c@d.com", result.Email);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsNull_WhenResponseIsNotSuccessful()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            var httpClient = GetHttpClient(response, out _);
            var apiClient = new ApiClient(httpClient, GetOptions());

            // Act
            var result = await apiClient.GetUserByIdAsync(99);

            // Assert
            Assert.IsNull(result);
        }
    }

    public class UsersResponse
    {
        public IEnumerable<UserInformationApp.Domain.User>? Data { get; set; }
    }

    public class UserResponse
    {
        public UserInformationApp.Domain.User? Data { get; set; }
    }
}
