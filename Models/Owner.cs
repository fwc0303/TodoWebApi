namespace TodoWebApi.Models
{
    public class Owner
    {
        public string Email { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; } = DateTime.Now;
        public DateTime TokenExpires { get; set; } = DateTime.Now;
    }
}
