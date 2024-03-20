using CatenaccioStore.Core.Entities.Identities;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
