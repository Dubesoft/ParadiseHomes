using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParadiesHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParadiesHome.Data;
using Microsoft.EntityFrameworkCore;

namespace ParadiesHome.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountController(ApplicationDbContext db,
                                 UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {


            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (ModelState.IsValid)
            {
                var userModel = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(userModel, model.Password);

                if (result.Succeeded)
                {

                    await _signInManager.SignInAsync(userModel, isPersistent: false);
                    return Redirect("/Customer/Home/Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl)
        {
            var roleName = "";
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        Redirect(ReturnUrl);
                    }
                    else
                    {
                        var userResult = await _db.Users.Where(u => u.Email == model.Email).FirstOrDefaultAsync();
                        var roles = await _userManager.GetRolesAsync(userResult);

                        foreach (var role in roles)
                        {
                            roleName = role.ToString();
                        }

                        if (roleName.Equals("Admin"))
                        {
                            return Redirect("/Admin/Administration/Index");
                        }
                        return Redirect("/Customer/Home/Index");
                    }

                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }

            return RedirectToAction(nameof(Login));

        }

        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return Redirect("/Customer/Home/Index");
        }

        public IActionResult ErrorSomethingWentWrong()
        {
            return View();
        }
    }
}