using Microsoft.JSInterop;
using Moq;
using System.Security.Claims;
using System.Text.Json;

namespace MoneyFlowTests.Auth
{
    public class CustomAuthStateProviderTests
    {
        [Fact]
        public async Task GetAuthenticationStateAsync_WhenUserInLocalStorage_ReturnsAuthenticatedState()
        {
            var userInfo = new UserInfo { UserId = 1, Email = "test@mail.com" };
            var userJson = JsonSerializer.Serialize(userInfo);

            var jsRuntimeMock = new Mock<IJSRuntime>();
            jsRuntimeMock
                .Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>()))
                .ReturnsAsync(userJson);

            var provider = new CustomAuthStateProvider(jsRuntimeMock.Object);

            var authState = await provider.GetAuthenticationStateAsync();

            Assert.True(authState.User.Identity.IsAuthenticated);
            Assert.Equal("1", authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_WhenNoUserInLocalStorage_ReturnsAnonymousState()
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();
            jsRuntimeMock
                .Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>()))
                .ReturnsAsync((string)null);

            var provider = new CustomAuthStateProvider(jsRuntimeMock.Object);

            var authState = await provider.GetAuthenticationStateAsync();

            Assert.False(authState.User.Identity.IsAuthenticated);
        }
    }
}
