using System.ComponentModel.DataAnnotations;

namespace XTrendApp.Web.Models.User
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}