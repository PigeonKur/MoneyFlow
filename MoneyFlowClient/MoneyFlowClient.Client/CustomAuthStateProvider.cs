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
        await _jsRuntime.InvokeVoidAsync("console.log", $"Данные из localStorage: {userJson}");

        if (string.IsNullOrWhiteSpace(userJson))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
            new Claim(ClaimTypes.Role, userInfo.RoleId.ToString()) // Добавляем роль в claims
        };

            if (!string.IsNullOrEmpty(userInfo.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, userInfo.Email));
            }

            var identity = new ClaimsIdentity(claims, "auth");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (Exception ex)
        {
            await _jsRuntime.InvokeVoidAsync("console.error", $"Ошибка десериализации: {ex.Message}");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
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
    public int RoleId { get; set; }
    public string Name { get; set; }

}
