using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyCeima.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Street { get; set; } 
        public string? City { get; set; } 
        public string? State { get; set; }  
        public string? ZipCode { get; set; } 
        public string? UserImg { get; set; }

     }
}
