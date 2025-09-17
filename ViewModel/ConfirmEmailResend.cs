using System.ComponentModel.DataAnnotations;

namespace MyCeima.ViewModel
{
    public class ConfirmEmailResend
    {

            [Required]
            public string EmailOrUserName { get; set; } = string.Empty;
            public string? ApplicationUserID { get; set; } = string.Empty;
        
    }
}
