using System.Text.Json.Serialization;

namespace MoneyFlowClient.Client.Models
{
    public class UserLoginModel
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class AuthResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
    }
}
