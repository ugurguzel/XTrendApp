namespace XTrendApp.Web.Models.User
{
    public class UserListViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}