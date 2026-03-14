using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Пароль должен быть не короче 8 символов")]
        public string Password { get; set; }
    }
}
