using System.ComponentModel.DataAnnotations;

namespace ProjectPrn222.Models.DTO
{
    public class UserViewModel
    {
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        [Display(Name = "Tên người dùng")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Chọn vai trò của user.")]
        public string RoleName { get; set; }
    }
}
