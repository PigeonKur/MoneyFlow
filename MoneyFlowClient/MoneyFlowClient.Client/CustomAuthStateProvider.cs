using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;

    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userInfo");

        if (string.IsNullOrWhiteSpace(userJson))
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(anonymous);
        }

        var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson);
        if (userInfo is null)
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(anonymous);
        }

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
            new Claim(ClaimTypes.Email, userInfo.Email)
        }, "auth");

        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public async Task SetUserAsync(UserInfo userInfo)
    {
        var json = JsonSerializer.Serialize(userInfo);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userInfo", json);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userInfo");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

public class UserInfo
{
    public int UserId { get; set; }
    public string Email { get; set; }
}
