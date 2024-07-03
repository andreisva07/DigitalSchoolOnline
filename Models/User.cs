using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models
{
    public class User
    {
        public int Id {  get; set; }
        public string CNP { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string? ResetPasswordTOken { get; set; }
        public DateTime ResetPasswordExpiry {  get; set; }
    }
}
