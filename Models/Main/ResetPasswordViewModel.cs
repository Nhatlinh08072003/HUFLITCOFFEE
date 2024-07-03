using System.ComponentModel.DataAnnotations;
public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc.")]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Xác nhận mật khẩu mới là bắt buộc.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.")]
    public string? ConfirmNewPassword { get; set; }
}

