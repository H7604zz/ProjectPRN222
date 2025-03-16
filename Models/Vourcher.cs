using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPrn222.Models;

public partial class Vourcher
{
    public string VourcherId { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Mã giảm giá không được để trống.")]
    [RegularExpression(@"\S+", ErrorMessage = "Mã giảm giá không được chỉ chứa khoảng trắng.")]
    public string Code { get; set; } = null!;

    public int Discount { get; set; }

    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    [Required(ErrorMessage = "Ngày hết hạn là bắt buộc.")]
    [ExpiryDate]
    public DateOnly ExpiryDate { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Mức tối thiểu phải lớn hơn hoặc bằng 0.")]
    public float MinOrderValue { get; set; }

    public float? MaxDiscountAmount { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class ExpiryDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateOnly expiryDate)
        {
            var createdAtProperty = validationContext.ObjectType.GetProperty("CreatedAt");
            if (createdAtProperty != null)
            {
                var createdAtValue = (DateOnly)createdAtProperty.GetValue(validationContext.ObjectInstance)!;
                if (expiryDate < createdAtValue)
                {
                    return new ValidationResult("Ngày hết hạn phải lớn hơn hoặc bằng ngày tạo.");
                }
            }
        }
        return ValidationResult.Success;
    }
}
