namespace MoneyFlowClient.Client.Models
{
    public class UserRegisterModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; } = 2; 
        public string ImgUrl { get; set; } 

    }
}
