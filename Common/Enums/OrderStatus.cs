using System.ComponentModel.DataAnnotations;

namespace Common.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Đang chờ xác nhận")]
        Pending = 1,

        [Display(Name = "Đã xác nhận")]
        Confirmed = 2,

        [Display(Name = "Đang giao hàng")]
        Shipping = 3,

        [Display(Name = "Giao hàng thành công")]
        Success = 4,

        [Display(Name = "Đã hủy")]
        Canceled = 5
    }
}