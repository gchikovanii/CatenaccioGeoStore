using Microsoft.AspNetCore.Identity;

namespace CatenaccioStore.Core.Entities.Identities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public Address Address { get; set; }
    }
}
