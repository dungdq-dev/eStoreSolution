using System.ComponentModel.DataAnnotations;

namespace Common.Enums
{
    public enum Status
    {
        [Display(Name = "Không hoạt động")]
        InActive,

        [Display(Name = "Hoạt động")]
        Active,
    }
}