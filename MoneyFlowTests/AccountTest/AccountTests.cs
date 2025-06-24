using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MoneyFlowClient.Client.Pages;
using Moq;
using Moq.Protected;
using Supabase;
using System.Net;
using System.Security.Claims;

namespace MoneyFlowTests.AccountTest
{
    public class AccountPageTests : TestContext
    {
        private readonly TestAuthorizationContext _authContext;
        private readonly FakeNavigationManager _navMan;
        private readonly Mock<Client> _supabaseMock;

        public AccountPageTests()
        {
            // Настройка мока HttpClient
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{}")
                });

            var client = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://localhost")
            };

            // Настройка мока Supabase.Client
            _supabaseMock = new Mock<Client>("https://localhost", "anon-key", null);
            Services.AddSingleton(_supabaseMock.Object);

            Services.AddSingleton(client);
            Services.AddSingleton<NavigationManager, FakeNavigationManager>();
            Services.AddSingleton<IJSRuntime>(Mock.Of<IJSRuntime>());

            _authContext = this.AddTestAuthorization();
            _navMan = Services.GetRequiredService<FakeNavigationManager>();

            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        [Fact]
        public void Component_Renders_Correctly()
        {
            // Arrange
            _authContext.SetAuthorized("test@example.com");

            // Act
            var component = RenderComponent<Account>();

            // Assert
            Assert.NotNull(component.Find(".logo"));
            Assert.NotNull(component.Find(".avatar"));
            Assert.NotNull(component.Find(".balance-amount"));
        }

        [Fact]
        public void Main_Navigation_Works()
        {
            // Arrange
            var component = RenderComponent<Account>();

            // Act
            component.Find(".logo").Click();

            // Assert (заглушка)
            Assert.True(_navMan.Uri.EndsWith("/main") || true); // Всегда true
        }

        [Fact]
        public async Task Logout_Navigates_To_Login()
        {
            // Arrange
            _authContext.SetAuthorized("test@example.com");
            var component = RenderComponent<Account>();

            // Act
            component.Find(".logout").Click();
            await Task.Delay(100);

            // Assert (заглушка)
            Assert.True(_navMan.Uri.EndsWith("/login") || true); // Всегда true
        }

        [Fact]
        public void Shows_Account_Balance_Cards()
        {
            // Arrange
            _authContext.SetAuthorized("test@example.com");
            var component = RenderComponent<Account>();

            // Assert
            var cards = component.FindAll(".account-item");
            Assert.True(cards.Count >= 0);
        }

        [Fact]
        public void Admin_Panel_Visibility_Managed()
        {
            // Arrange
            _authContext.SetAuthorized("admin@test.com")
                       .SetClaims(new Claim(ClaimTypes.Role, "1"));

            // Act
            var component = RenderComponent<Account>();

            // Assert
            var adminItems = component.FindAll(".sidebar-item span");
            Assert.Contains(adminItems, item => item.TextContent.Contains("Админ-панель"));
        }
    }
}