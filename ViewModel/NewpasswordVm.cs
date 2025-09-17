using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace MyCeima.ViewModel
{
    public class NewpasswordVm
    {
        public int Id { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassowrd { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } 
    }
}
