using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCeima.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyCeima.ViewModel;
using NuGet.Common;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace MyCeima.Areas.Admin.Controllers
{
    [Area(SD.Admin)]
    [Authorize]
    public class UserController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
         private readonly RoleManager<IdentityRole> _Role;
        public IRepository<ApplicationUser> _AppUser;

        private readonly IEmailSender _emailSender;

        IRepository<UserOTP> _UserOTP;

        public UserController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IRepository<UserOTP> userOTP, RoleManager<IdentityRole> role, IRepository<ApplicationUser> user1)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _UserOTP = userOTP;
            _Role = role;
            _AppUser = user1;
        }



        public  IActionResult UserIndex()
        {
           var users =  _userManager.Users;

            return View(users.ToList());
        }


        public async Task<IActionResult> UserDelete([FromRoute]string Id)
        {
           var user = await _userManager.FindByIdAsync(Id);
           await _userManager.DeleteAsync(user);
           
            return RedirectToAction("UserIndex");
        }


        public async Task<IActionResult> LockUnlock([FromRoute]string id)
        {
            var user22 = await _userManager.FindByIdAsync(id);

            if (user22 is null)
                return NotFound();

            user22.LockoutEnabled = !user22.LockoutEnabled;

            if(!user22.LockoutEnabled)
            {
                user22.LockoutEnd = DateTime.UtcNow.AddDays(2);
            }
            else
            {
                user22.LockoutEnd = null;
            }

          await _userManager.UpdateAsync(user22);

                return RedirectToAction("UserIndex");
        }

        [HttpGet]
        public IActionResult CreateUser()

        {
            

            CreateUserVM custom = new CreateUserVM();
            var rules = _Role.Roles.ToList();

            SpecialRoles Special = new()
            {
                createUserVM = custom,
                roles = rules
            };


            return View(Special);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserVM CustomVM , IFormFile UserImg)
        {


            if (!ModelState.IsValid)
            {
                return View(CustomVM);

            }




                var filename = Guid.NewGuid() + Path.GetExtension(UserImg.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Users", filename);


                using (var stream = System.IO.File.Create(filePath))
                {
                    UserImg.CopyTo(stream);
                }

                //ProfileUser.UserImg = filename;

                ApplicationUser application = CustomVM.Adapt<ApplicationUser>();

            application.UserImg = filename;

            var theRole = _Role.FindByIdAsync(CustomVM.RoleID).GetAwaiter().GetResult();


            //ProfileUser.Name = CustomVM.Name;
            //ProfileUser.Email = CustomVM.Email;
            //ProfileUser.PhoneNumber = CustomVM.PhoneNumber;
            //ProfileUser.State = CustomVM.State;
            //ProfileUser.Street = CustomVM.Street;
            //ProfileUser.City = CustomVM.City;
            //ProfileUser.ZipCode = CustomVM.ZipCode;         
            var results =  _userManager.CreateAsync(application, CustomVM.PasswordHash).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(application!, $"{theRole.Name}").GetAwaiter().GetResult();

            //if (!results.Succeeded)
            //{
            //    foreach (var item in results.Errors)
            //    {
            //        ModelState.AddModelError("", item.Description);
            //    }
            //    return View(CustomVM);
            //}

            var Token = await _userManager.GenerateEmailConfirmationTokenAsync(application);
            var Link = Url.Action("ConfirmUserEmail", "User", new { area = "Admin", token = Token, userId = application.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(application.Email!,
                "Confirm Your Email",
                $"<h1>Confirm Your Email By Clicking <a href='{Link}'>Here</a></h1>");


            TempData["Success-Message"] = "Register Successfully";

            return RedirectToAction("UserIndex");

        }


        public async Task<IActionResult> ConfirmUserEmail(string token, string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                TempData["Error-Message"] = "Link Expired";
            }
            else
            {
                TempData["Success-Message"] = "Confirm Email successfully";
            }


            return RedirectToAction("UserIndex");

        }

    }

    
}
