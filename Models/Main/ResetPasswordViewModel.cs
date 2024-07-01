using System.ComponentModel.DataAnnotations;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string ConfirmNewPassword { get; set; }
}
