using System.ComponentModel.DataAnnotations;

namespace HUFLITCOFFEE.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        // Các thông tin khác của người dùng mà bạn muốn hiển thị trên profile
    }
}