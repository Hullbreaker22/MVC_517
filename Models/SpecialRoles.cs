using Microsoft.AspNetCore.Identity;

namespace MyCeima.Models
{
    public class SpecialRoles
    {
        public CreateUserVM createUserVM { get; set; }

        public List<IdentityRole> roles { get; set; }

    }
}
