using Xunit;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using MoneyFlowClient.Client.Pages;
using Bunit.JSInterop;
using Bunit.TestDoubles;
using Microsoft.JSInterop;
using Moq;

public class TransfersTests : TestContext
{
    public TransfersTests()
    {
        this.AddTestAuthorization();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void GetInitials_ReturnsCorrectInitials()
    {
        // Arrange
        var component = new Transfers();

        // Act & Assert
        Assert.Equal("JD", component.GetInitials("John Doe"));
        Assert.Equal("JO", component.GetInitials("John O'Neil"));
        Assert.Equal("??", component.GetInitials(""));
    }

    [Fact]
    public void GetShortName_ReturnsCorrectShortName()
    {
        // Arrange
        var component = new Transfers();

        // Act & Assert
        Assert.Equal("John D.", component.GetShortName("John Doe"));
        Assert.Equal("Verylong...", component.GetShortName("Verylongname"));
        Assert.Equal("??", component.GetShortName(""));
    }

    [Fact]
    public async Task HandleTransfer_ShowsAlert_WhenCalled()
    {
        // Arrange
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("test@test.com");
        authContext.SetClaims(
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, "test@test.com")
        );

        Services.AddSingleton<IJSRuntime>(JSInterop.JSRuntime); // Подключаем JSInterop

        JSInterop.SetupVoid("alert"); // заглушка на alert

        var component = RenderComponent<Transfers>();
        component.Instance.transferTarget = "test@test.com";

        // Act
        await component.InvokeAsync(() => component.Instance.HandleTransfer());

    }

    [Fact]
    public async Task ExecuteTransfer_ShowsAlert_WhenCalled()
    {
        // Arrange
        Services.AddSingleton<IJSRuntime>(JSInterop.JSRuntime);

        JSInterop.SetupVoid("alert"); // заглушка на alert

        var component = RenderComponent<Transfers>();
        component.Instance.transferAmount = -100;
        component.Instance.targetUser = new Transfers.User { UserId = 2 };

        // Act
        await component.InvokeAsync(() => component.Instance.ExecuteTransfer());

    }


}
