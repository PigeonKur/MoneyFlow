using Xunit;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using MoneyFlowClient.Client.Pages;
using Bunit.JSInterop;
using Bunit.TestDoubles;
using Microsoft.JSInterop;
using Moq;
using Supabase;

namespace MoneyFlowTests.TransfersTest
{
    public class TransfersTests : TestContext
    {
        private readonly Mock<Client> _supabaseMock;

        public TransfersTests()
        {
            _supabaseMock = new Mock<Client>("https://localhost", "anon-key", null);
            Services.AddSingleton(_supabaseMock.Object);

            this.AddTestAuthorization();
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        [Fact]
        public void GetInitials_ReturnsCorrectInitials()
        {
            var component = new Transfers();

            Assert.Equal("JD", component.GetInitials("John Doe"));
            Assert.Equal("JO", component.GetInitials("John O'Neil"));
            Assert.Equal("??", component.GetInitials(""));
        }

        [Fact]
        public void GetShortName_ReturnsCorrectShortName()
        {
            var component = new Transfers();

            Assert.Equal("John D.", component.GetShortName("John Doe"));
            Assert.Equal("Verylong...", component.GetShortName("Verylongname"));
            Assert.Equal("??", component.GetShortName(""));
        }

        [Fact]
        public async Task HandleTransfer_ShowsAlert_WhenCalled()
        {
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("test@test.com");
            authContext.SetClaims(
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Email, "test@test.com")
            );

            Services.AddSingleton<IJSRuntime>(JSInterop.JSRuntime);
            JSInterop.SetupVoid("alert");

            var component = RenderComponent<Transfers>();
            component.Instance.transferTarget = "test@test.com";

             
            await component.InvokeAsync(() => component.Instance.HandleTransfer());
        }

        [Fact]
        public async Task ExecuteTransfer_ShowsAlert_WhenCalled()
        {
            Services.AddSingleton<IJSRuntime>(JSInterop.JSRuntime);
            JSInterop.SetupVoid("alert");

            var component = RenderComponent<Transfers>();
            component.Instance.transferAmount = -100;
            component.Instance.targetUser = new Transfers.User { UserId = 2 };

            await component.InvokeAsync(() => component.Instance.ExecuteTransfer());
        }
    }
}