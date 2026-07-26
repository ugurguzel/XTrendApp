using System.ComponentModel.DataAnnotations;

namespace XTrendApp.Web.Models.User
{
    public class UserPasswordViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}