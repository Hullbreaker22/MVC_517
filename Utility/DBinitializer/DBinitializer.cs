using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyCeima.Models;

namespace MyCeima.Utility.DBinitializer
{
    public class DBinitializer : IDBinitializer
    {
        private readonly ApplicationDBContext _Context;
        private readonly RoleManager<IdentityRole> _Role;
        private readonly UserManager<ApplicationUser> _userManager;


        public DBinitializer(ApplicationDBContext context, RoleManager<IdentityRole> role , UserManager<ApplicationUser> usermanager)
        {
            _Context = context;
            _Role = role;
            _userManager = usermanager;
        }

        public void initialize()
        {
            if (_Context.Database.GetPendingMigrations().Any())
            {
                _Context.Database.Migrate();
            }

            if (_Role.Roles.IsNullOrEmpty())
            {
                _Role.CreateAsync(new(SD.SuperAdmin)).GetAwaiter().GetResult();
                _Role.CreateAsync(new(SD.Admin)).GetAwaiter().GetResult();
                _Role.CreateAsync(new(SD.Company)).GetAwaiter().GetResult();
                _Role.CreateAsync(new(SD.Customer)).GetAwaiter().GetResult();


                _userManager.CreateAsync(new()
                {

                    Email = "SuperAdmin@Gmail.com",
                    EmailConfirmed = true,
                    UserName = "AdminUser",
                    Name = "Super Add"
                }, "Admin123@").GetAwaiter().GetResult();

                var User = _userManager.FindByEmailAsync("SuperAdmin@Gmail.com").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(User!,SD.SuperAdmin).GetAwaiter().GetResult();

            }

        }

    }
}
