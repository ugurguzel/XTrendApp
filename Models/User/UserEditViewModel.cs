using System.ComponentModel.DataAnnotations;

namespace XTrendApp.Web.Models.User
{
    public class UserEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FullName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsActive { get; set; }
    }
}