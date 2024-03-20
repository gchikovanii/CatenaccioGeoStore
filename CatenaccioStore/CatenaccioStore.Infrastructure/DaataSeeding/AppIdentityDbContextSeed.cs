using CatenaccioStore.Core.Entities.Identities;
using Microsoft.AspNetCore.Identity;

namespace CatenaccioStore.Infrastructure.DaataSeeding
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Bob",
                    Email = "Bob@gmail.com",
                    UserName = "BOByyyy",
                    Address = new Address
                    {
                        FirstName = "Bob",
                        LastName = "Bibba",
                        Street = "Street of fighters 55",
                        City = "Tbilisi",
                        ZipCode = "0163"
                    }
                };
                await userManager.CreateAsync(user,"Vaisheyleo_1!");
            }
        }
    }
}
