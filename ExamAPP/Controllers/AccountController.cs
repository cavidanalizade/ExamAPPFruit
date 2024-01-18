using ExamAPP.Helper;
using ExamAPP.Models;
using ExamAPP.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPP.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly SignInManager<AppUser> _signinmanager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> usermanager, SignInManager<AppUser> signinmanager, RoleManager<IdentityRole> roleManager)
        {
            _usermanager = usermanager;
            _signinmanager = signinmanager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVM registerUserVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerUserVM);
            }
            AppUser appUser = new AppUser()
            {
                Name = registerUserVM.Name,
                Surname = registerUserVM.Surname,
                Email = registerUserVM.Email,
                UserName = registerUserVM.Username
            };
            var create = await _usermanager.CreateAsync(appUser, registerUserVM.Password);
            if (!create.Succeeded)
            {
                foreach (var item in create.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(registerUserVM);
            }
            await _usermanager.AddToRoleAsync(appUser, UserRole.Admin.ToString());
            return RedirectToAction(nameof(Login));

        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            AppUser user = await _usermanager.FindByEmailAsync(loginVM.Email);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Email or passwor is invalid");
                return View(loginVM);
            }

            var result = await _signinmanager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your accout is locked out");
                return View(loginVM);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email or passwor is invalid");
                return View(loginVM);
            }
            return RedirectToAction(nameof(Index), "Home");

        }
        public async Task<IActionResult> Logout()
        {
            _signinmanager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");

        }
        public async Task<IActionResult> CreateRole()
        {
            foreach (UserRole item in Enum.GetValues(typeof(UserRole)))
            {
                if (await _roleManager.FindByNameAsync(item.ToString()) == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = item.ToString(),
                    });
                }
            }
            return RedirectToAction(nameof(Index), "Home");

        }
    }
}
