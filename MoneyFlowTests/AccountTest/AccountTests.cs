using Xunit;
using Bunit;
using Microsoft.AspNetCore.Components;
using MoneyFlowClient.Client.Pages;
using Bunit.TestDoubles;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Security.Claims;
using Bunit.JSInterop;
using Moq;

namespace MoneyFlowTests.AccountTest
{
    public class AccountPageTests : TestContext
    {
        private readonly TestAuthorizationContext _authContext;
        private readonly FakeNavigationManager _navMan;

        public AccountPageTests()
        {
            // Настройка поддельного HttpClient
            var httpClientMock = new Mock<HttpClient>();
            Services.AddSingleton(httpClientMock.Object);

            // Настройка авторизации
            _authContext = this.AddTestAuthorization();
            _navMan = Services.GetRequiredService<FakeNavigationManager>();

            // Настройка JSInterop
            JSInterop.Mode = JSRuntimeMode.Loose;

            // Мокируем успешные ответы API
            httpClientMock.Setup(x => x.GetAsync(It.IsAny<string>()))
                         .ReturnsAsync(new HttpResponseMessage
                         {
                             StatusCode = System.Net.HttpStatusCode.OK,
                             Content = new StringContent("{}")
                         });
        }

        [Fact]
        public void Component_Renders_Correctly()
        {
            // Arrange & Act
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

            // Assert
            Assert.EndsWith("/main", _navMan.Uri);
        }

        [Fact]
        public async Task Logout_Navigates_To_Login()
        {
            // Arrange
            _authContext.SetAuthorized("test@example.com");
            var component = RenderComponent<Account>();

            // Act
            await component.InvokeAsync(() =>
                component.Find(".logout").Click());

            // Assert
            Assert.EndsWith("/login", _navMan.Uri);
        }

        [Fact]
        public void Displays_User_Profile_Info()
        {
            // Arrange & Act
            var component = RenderComponent<Account>();

            // Assert
            var userName = component.Find("h2").TextContent;
            Assert.False(string.IsNullOrEmpty(userName));
        }

        [Fact]
        public void Shows_Account_Balance_Cards()
        {
            // Arrange & Act
            var component = RenderComponent<Account>();

            // Assert
            var cards = component.FindAll(".account-item");
            Assert.True(cards.Count >= 1);
        }

        [Fact]
        public void Admin_Panel_Visibility_Managed()
        {
            // Arrange - Admin
            _authContext.SetAuthorized("admin@test.com")
                      .SetClaims(new Claim(ClaimTypes.Role, "1"));
            var adminComponent = RenderComponent<Account>();

            // Assert
            var adminItems = adminComponent.FindAll(".sidebar-item span");
            Assert.True(adminItems.Count > 0);
        }

       
    }
}