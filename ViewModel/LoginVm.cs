using System.ComponentModel.DataAnnotations;

namespace MyCeima.ViewModel
{
    public class LoginVm
    {
        [Required,DataType(DataType.EmailAddress)]
        public string EmailOrUserName { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string  Password{ get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
