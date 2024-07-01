using System.ComponentModel.DataAnnotations;

namespace HUFLITCOFFEE.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

    }
}