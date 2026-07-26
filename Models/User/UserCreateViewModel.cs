using System.ComponentModel.DataAnnotations;

namespace XTrendApp.Web.Models.User
{
    public class UserCreateViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsActive { get; set; } = true;
    }
}