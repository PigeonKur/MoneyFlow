using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MoneyFlowClient.Client.Pages;
using Microsoft.JSInterop;
using Supabase;

namespace MoneyFlowTests.HistoryTest
{
    public class HistoryTests : TestContext
    {
        private readonly Mock<IJSRuntime> _jsRuntimeMock;
        private readonly Mock<AuthenticationStateProvider> _authStateProviderMock;
        private readonly Mock<HttpClient> _httpClientMock;
        private readonly TestNavigationManager _navigationManager;
        private readonly Mock<Client> _supabaseMock;

        public HistoryTests()
        {
            _supabaseMock = new Mock<Client>("https://localhost", "anon-key", null);
            Services.AddSingleton(_supabaseMock.Object);

            _jsRuntimeMock = new Mock<IJSRuntime>();
            _authStateProviderMock = new Mock<AuthenticationStateProvider>();
            _httpClientMock = new Mock<HttpClient>();

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Role, "0")
            }, "test");
            var user = new ClaimsPrincipal(identity);

            var authState = new AuthenticationState(user);
            _authStateProviderMock.Setup(x => x.GetAuthenticationStateAsync())
                .ReturnsAsync(authState);

            Services.AddSingleton(_authStateProviderMock.Object);
            Services.AddSingleton(_jsRuntimeMock.Object);
            Services.AddSingleton(_httpClientMock.Object);

            _navigationManager = new TestNavigationManager();
            Services.AddSingleton<NavigationManager>(_navigationManager);
        }

        [Fact]
        public void Component_Renders_Correctly()
        {
            var component = RenderComponent<History>();

            Assert.Contains("История операций", component.Markup);
            Assert.Contains("Неделя", component.Markup);
            Assert.Contains("Месяц", component.Markup);
            Assert.Contains("Все типы", component.Markup);
        }

        [Fact]
        public void GetTransactionDirectionSign_ReturnsCorrectSign()
        {
            var component = RenderComponent<History>();
            var transaction = new History.TransactionDto { UserId = 123 };

            Assert.Equal("−", component.Instance.GetTransactionDirectionSign(transaction));
        }

        [Fact]
        public void GetTransactionCounterpartyLabel_ReturnsCorrectLabel()
        {
            var component = RenderComponent<History>();
            var transaction = new History.TransactionDto { UserId = 123 };

            Assert.Equal("Получатель", component.Instance.GetTransactionCounterpartyLabel(transaction));
        }

        [Fact]
        public void PeriodFilter_WorksCorrectly()
        {
            var component = RenderComponent<History>();

            component.FindAll("select")[0].Change("year");

            Assert.Equal("year", component.Instance.selectedPeriod);
        }
    }

    public class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager()
        {
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            Uri = uri;
        }
    }
}