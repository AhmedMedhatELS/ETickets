using ETickets.Models;
using ETickets.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;

namespace ETickets.Controllers
{
    public class AccountController : Controller
    {
        #region Services Handling
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        #endregion
        #region Register
        public IActionResult Register() => View();
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ApplicationUserVM userVM)
        {
            if(ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    FirstName = userVM.FirstName,
                    LastName = userVM.LastName,
                    UserName = $"{userVM.FirstName?.Trim()}_{userVM.LastName?.Trim()}",
                    Email = userVM.Email,
                    DateofBirth = userVM.DateBirth,
                    PhoneNumber = userVM.Phone,
                    Gender = userVM.Gender,
                };
                
                if ((await userManager.CreateAsync(user, userVM.Password)).Succeeded)
                {
                    if (User.IsInRole("Admin"))
                        await userManager.AddToRoleAsync(user, "Admin");
                    else
                    {
                        await userManager.AddToRoleAsync(user, "Customer");
                        await signInManager.SignInAsync(user, userVM.RememberMe);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("Password", "Password Didn't Match The constraints!!");
                
            }
            return View(userVM);
        }
        #endregion
        #region Login
        public async Task<IActionResult> Login()
        {
            if (roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new("Admin"));
                await roleManager.CreateAsync(new("Customer"));
                await roleManager.CreateAsync(new("Cinema"));
                var adminUser = new ApplicationUser
                {
                    FirstName = "Ahmed",
                    LastName = "Medhat",
                    UserName = "Ahmed_Admin",
                    Email = "ahmed@admin.com",
                    DateofBirth = new DateTime(1998, 12, 5),
                    PhoneNumber = "01099095817",
                    Gender = 0,
                };

                if ((await userManager.FindByEmailAsync(adminUser.Email)) == null)
                    if ((await userManager.CreateAsync(adminUser, "@hmed1998Admin")).Succeeded)
                        await userManager.AddToRoleAsync(adminUser, "Admin");

            }
            
            return View();
        }      
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserVM userVM)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(userVM.Email);

                if (user != null && await userManager.CheckPasswordAsync(user, userVM.Password))
                {
                    await signInManager.SignInAsync(user, userVM.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("EorP", "Incorrect Email or Password");
            return View(userVM);
        }
        #endregion
        #region Logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        #endregion
    }
}
