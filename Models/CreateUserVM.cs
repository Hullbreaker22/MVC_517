using System.ComponentModel.DataAnnotations;

namespace MyCeima.Models
{
    public class CreateUserVM
    {
        [Required()]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required,DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required,DataType(DataType.Password)]
        public string PasswordHash { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), Compare(nameof(PasswordHash))]
        public string ConfirmPasswordHash { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? UserImg { get; set; }
        public string? RoleID { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
