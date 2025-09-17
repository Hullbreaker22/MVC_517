using Humanizer;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCeima.Models;
using MyCeima.ViewModel;
using System.Threading.Tasks;

namespace MyCeima.Areas.Identity.Controllers
{

    [Area(SD.IdentityArea)]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IRepository<ApplicationUser> _AppUser;

        public ProfileController(UserManager<ApplicationUser> user, IRepository<ApplicationUser> appUser)
        {
            _userManager = user;
            _AppUser = appUser;
        }

        [HttpGet]
        public async Task<IActionResult> ProfileIndex()
        {
            var ProfileUser = await _userManager.GetUserAsync(User);

            return View(ProfileUser);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileIndex(ProfileVM profileVM , IFormFile UserImg)
        {

            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}

            var ProfileUser = await _userManager.GetUserAsync(User);
            var app2 = await _AppUser.GetOne(Expression: e => e.Id == ProfileUser.Id, asNoTracking: true);
            
            if (UserImg is not null)
            {
                var filename = Guid.NewGuid() + Path.GetExtension(UserImg.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\Users",filename);

              
                using (var stream = System.IO.File.Create(filePath))
                {
                    UserImg.CopyTo(stream);
                }

                ProfileUser.UserImg = filename;

                if(app2.UserImg is not null)
                {
                    var filePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Users", app2.UserImg);
                    if (System.IO.File.Exists(filePath2))
                    {
                        System.IO.File.Delete(filePath2);
                    }
                }
                      

            }
            else
            {
                ProfileUser.UserImg = app2.UserImg;
            }

          

            ProfileUser.Name = profileVM.Name;
            ProfileUser.Email = profileVM.Email;
            ProfileUser.PhoneNumber = profileVM.PhoneNumber;
            ProfileUser.State = profileVM.State;
            ProfileUser.Street = profileVM.Street;
            ProfileUser.City = profileVM.City;
            ProfileUser.ZipCode = profileVM.ZipCode;
            var token = await _userManager.GeneratePasswordResetTokenAsync(ProfileUser);
            await _userManager.ResetPasswordAsync(ProfileUser, token, profileVM.PasswordHash);

            await _userManager.UpdateAsync(ProfileUser); 

            return RedirectToAction("ProfileIndex");
        }



     
    }
}
