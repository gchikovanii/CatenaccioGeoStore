using CatenaccioStore.Core.DTOs.Security;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IPasswordRecoveryTokenService
    {
        Task<TokenDto> GetToken(string userEmail, CancellationToken cancellationToken);
        Task<bool> CreateAsync(CancellationToken cancellationToken, TokenDto token);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, string userEmail);
    }
}
