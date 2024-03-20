using System.ComponentModel.DataAnnotations;

namespace CatenaccioStore.API.Infrastructure.Models
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Should have at least one lowercase letter, one uppercase letter, one digit, one special character, and minimum 8 characters")]
        public string Password { get; set; }
    }
}
