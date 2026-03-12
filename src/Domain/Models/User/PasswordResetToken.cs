using System.Security.Cryptography;

namespace Domain.Models.User
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }

        public int Token { get; set; } = RandomNumberGenerator.GetInt32(100000, 1000000);
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}
