using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MyCeima.Models;
using MyCeima.ViewModel;
using NuGet.Common;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MyCeima.Areas.Identity.Controllers
{
    [Area(SD.IdentityArea)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _Signmanager;
        private readonly IEmailSender _emailSender;

        IRepository<UserOTP> _UserOTP;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signmanager, IRepository<UserOTP> userOTP)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _Signmanager = signmanager;
            _UserOTP = userOTP;
        }

        [HttpGet]
        public IActionResult Index()

        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }


            CustomVM custom = new CustomVM();

            return View(custom);
        }


        [HttpPost]
        public async Task<IActionResult> Index(CustomVM CustomVM)
        {

          

            if (!ModelState.IsValid)
            {
                return View(CustomVM);
            }

            ApplicationUser application = CustomVM.Adapt<ApplicationUser>();

           var results = await _userManager.CreateAsync(application, CustomVM.PasswordHash);
           
            if(!results.Succeeded)
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(CustomVM);                
            }

            var Token = await _userManager.GenerateEmailConfirmationTokenAsync(application);
            var Link =  Url.Action("ConfirmEmail", "Account" ,new {area = "Identity" , token = Token, userId = application.Id } , Request.Scheme );
           
            await _emailSender.SendEmailAsync(application.Email!,
                "Confirm Your Email",
                $"<h1>Confirm Your Email By Clicking <a href='{Link}'>Here</a></h1>");


            TempData["Success-Message"] = "Register Successfully";

            return RedirectToAction("Login", "Account", new { area = "Identity"});
         
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
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


            return RedirectToAction("Index", "Home", new { area = "Customer" });

        }

        [HttpGet]
        public IActionResult Login()
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm loginVm)
        {


            if (!ModelState.IsValid)
            {
               return View(loginVm);
            }

            var user = await _userManager.FindByEmailAsync(loginVm.EmailOrUserName) ?? await _userManager.FindByNameAsync(loginVm.EmailOrUserName);

            if(user is null)
            {
                TempData["Error-Message"] = "Invalid User/Password";
                return View(loginVm);
            }


            var results = await  _Signmanager.PasswordSignInAsync(user,loginVm.Password,loginVm.RememberMe, true);

            if (!results.Succeeded)
            {
                if (results.IsLockedOut)
                {
                    TempData["Error-Message"] = "Too many attempts, your account is locked.";
                }
                else if (results.IsNotAllowed)
                {
                    TempData["Error-Message"] = "You are not allowed to log in.";
                }
                else
                {
                    
                    TempData["Error-Message"] = "Invalid username or password.";
                }

                return View(loginVm);
            }

            if (!user.EmailConfirmed)
            {
                
                    TempData["Error-Message"] = "Confirm your Email first";

                return View(loginVm);

            }
                 TempData["Success-Message"] = "Login Successfully";


            return RedirectToAction("Index", "Home", new {area = "Customer"});
            }


        [HttpGet]
        public IActionResult ResentEmail()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResentEmail(ConfirmEmailResend ConfirmEmailResend)
        {
            if(!ModelState.IsValid)
            {
                return View(ConfirmEmailResend);
            }

            var user = await _userManager.FindByEmailAsync(ConfirmEmailResend.EmailOrUserName) ?? await _userManager.FindByNameAsync(ConfirmEmailResend.EmailOrUserName);
            if (user is null)
            {
                TempData["Error-Message"] = "Invalid User ";
                return View(ConfirmEmailResend); 
            }


            if (user.EmailConfirmed)
            {
                TempData["Error-Message"] = "Already Confirmed";
                return View(ConfirmEmailResend);
            }


            var Token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var Link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", token = Token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email,
                "Confirm Your Email",
                $"<h1>Confirm Your Email By Clicking <a href='{Link}'>Here</a></h1>");


            TempData["Success-Message"] = "Send Successfully";

            return RedirectToAction("Login", "Account", new { area = "Identity" });

        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {

            return View();
         }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ConfirmEmailResend Foregetpassword)
        {
            if (!ModelState.IsValid)
            {
                return View(Foregetpassword);
            }

            var user = await _userManager.FindByEmailAsync(Foregetpassword.EmailOrUserName) ?? await _userManager.FindByNameAsync(Foregetpassword.EmailOrUserName);

            if (user is null)
            {
                TempData["Error-Message"] = "Invalid Email ";
                return View(Foregetpassword);
            }


            if (user.EmailConfirmed)
            {
                TempData["Error-Message"] = "Already Confirmed";
                return View(Foregetpassword);
            }

            
            var OTPToken = new Random().Next(1000, 9999);
            var Link = Url.Action("ResetPassword", "Account", new { area = "Identity", token = OTPToken, userId = user.Id }, Request.Scheme);

            await _UserOTP.CreateAsync(new()
            {
                ApplicationUserId = user.Id,
                OTPnumber = OTPToken.ToString(),
                ValidTO = DateTime.UtcNow.AddDays(1)

            });
            await _UserOTP.Commit();


            await _emailSender.SendEmailAsync(user.Email!,
                "Confirm Your Email",
                $"<h1>Reset Password Using this Code {OTPToken} </h1>");


            TempData["Success-Message"] = "Send Successfully";

            return RedirectToAction("ResetPassword", "Account", new { area = "Identity" , Userid = user.Id });

        }


        [HttpGet]
        public IActionResult ResetPassword(string Userid)
        {

            return View(new ResetPasswordVM()
            {
                ApplicationUserID = Userid,
            });
        }



        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetpassword)
        {


            var user = await _userManager.FindByIdAsync(resetpassword.ApplicationUserID) ?? await _userManager.FindByNameAsync(resetpassword.ApplicationUserID);

            if (user is null)
            {
                TempData["Error-Message"] = "Invalid User or Email ";
                return View(resetpassword);
            }

            var resultOTP = (await _UserOTP.GetAllAsync(e => e.ApplicationUserId == resetpassword.ApplicationUserID)).OrderBy(e => e.Id).LastOrDefault();

            if (resultOTP.OTPnumber != resetpassword.OTPnumber)
            {
                TempData["Error-Message"] = "Invalid OTP ";
                return View(resetpassword);
            }

            TempData["Success-Message"] = "Send Successfully";

            return RedirectToAction("NewPassword", "Account", new { area = "Identity", Userid = user.Id });
        
        }
        
        [HttpGet]
        public IActionResult NewPassword(string Userid)
        {

            return View(new NewpasswordVm()
            {
                ApplicationUserId = Userid
            });

        }



        [HttpPost]
        public async Task<IActionResult> NewPassword(NewpasswordVm newpasswordVm)
        {
          

            var user = await _userManager.FindByIdAsync(newpasswordVm.ApplicationUserId) ?? await _userManager.FindByNameAsync(newpasswordVm.ApplicationUserId);

            if (user is null)
            {
                TempData["Error-Message"] = "Invalid User name or Email";
                return View(newpasswordVm);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user,token, newpasswordVm.Password);

            TempData["Success-Message"] = "Changed Successfully";

            return RedirectToAction("Login", "Account", new { area = "Identity", Userid = user.Id });

        }


        public IActionResult LogOut()
        {
            _Signmanager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Identity"});
        }

        public IActionResult AccessDeniedPath()
        {
            return View();
        }


     

    }
}
