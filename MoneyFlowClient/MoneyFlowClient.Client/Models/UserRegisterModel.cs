namespace MoneyFlowClient.Client.Models
{
    public class UserRegisterModel
    {
        public int UserId { get; set; } = 8;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; } = 1; // например дефолтная роль
    }
}
