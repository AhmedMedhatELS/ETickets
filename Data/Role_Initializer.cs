using Microsoft.AspNetCore.Identity;
using ETickets.Models.ViewModel;
using ETickets.Models;
namespace ETickets.Data
{
    public static class Role_Initializer
    {
        public static async void InitializeAsync(IServiceProvider serviceProvider)
        {
            //using (var scope = serviceProvider.CreateScope())
            //{
            //    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleVM>>();
            //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //    string[] roleNames = { "Admin", "Cinema", "User" };
            //    foreach (var roleName in roleNames)               
            //        if (!roleManager.RoleExistsAsync(roleName).Result)
            //            roleManager.CreateAsync(new RoleVM(roleName)).Wait();

            //    var adminUser = new ApplicationUser
            //    {
            //        FirstName = "Ahmed",
            //        LastName = "Medhat",
            //        UserName = "Ahmed_Admin",
            //        Email = "ahmed@admin.com",
            //        DateofBirth = new DateTime(1998, 12, 5),
            //        PhoneNumber = "01099095817",
            //        Gender = 0,
            //    };
               
            //    if (userManager.FindByEmailAsync(adminUser.Email).Result == null)
            //    {
            //        var result = await userManager.CreateAsync(adminUser, "@hmed1998Admin");
            //        if (result.Succeeded)
            //        {
            //            await userManager.AddToRoleAsync(adminUser, "Admin");
            //        }
            //    }
            //}
        }
    }
}
