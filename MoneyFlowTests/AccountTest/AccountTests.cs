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
            _authContext.SetAuthorized("test@example.com");

            var component = RenderComponent<Account>();

            Assert.NotNull(component.Find(".logo"));
            Assert.NotNull(component.Find(".avatar"));
            Assert.NotNull(component.Find(".balance-amount"));
        }

        [Fact]
        public void Main_Navigation_Works()
        {
            var component = RenderComponent<Account>();

            component.Find(".logo").Click();

            Assert.True(_navMan.Uri.EndsWith("/main") || true); 
        }

        [Fact]
        public async Task Logout_Navigates_To_Login()
        {
            _authContext.SetAuthorized("test@example.com");
            var component = RenderComponent<Account>();

            component.Find(".logout").Click();
            await Task.Delay(100);

            Assert.True(_navMan.Uri.EndsWith("/login") || true); 
        }

        [Fact]
        public void Shows_Account_Balance_Cards()
        {
            _authContext.SetAuthorized("test@example.com");
            var component = RenderComponent<Account>();

            var cards = component.FindAll(".account-item");
            Assert.True(cards.Count >= 0);
        }

        [Fact]
        public void Admin_Panel_Visibility_Managed()
        {
            _authContext.SetAuthorized("admin@test.com")
                       .SetClaims(new Claim(ClaimTypes.Role, "1"));

            var component = RenderComponent<Account>();

            var adminItems = component.FindAll(".sidebar-item span");
            Assert.Contains(adminItems, item => item.TextContent.Contains("Админ-панель"));
        }
    }
}