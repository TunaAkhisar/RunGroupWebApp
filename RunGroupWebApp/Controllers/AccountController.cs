using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Models;
using RunGroupWebApp.Data;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVm)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVm);
            }

            var user = await _userManager.FindByEmailAsync(loginVm.EmailAddress);
            
            if(user != null)
            {
                //User is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user,loginVm.Password);

                if (passwordCheck)
                {
                    //Password correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginVm.Password, false, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Race");
                    }
                }
                //Password is incorrect
                TempData["Error"] = "Wrong credentials. Please try again";
                return View(loginVm);
            }
            //User not found
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginVm);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);

            if(user != null )
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerVM);
            }

            var newUser = new AppUser()
            {
                Email = registerVM.EmailAddress,
                UserName = registerVM.EmailAddress
            };

            var newUserResponse = await _userManager.CreateAsync(newUser,registerVM.Password);

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            }

            return RedirectToAction("Index", "Race");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Race");
        }


    }
}
