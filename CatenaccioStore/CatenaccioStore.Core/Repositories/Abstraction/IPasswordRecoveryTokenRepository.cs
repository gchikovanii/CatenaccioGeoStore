using CatenaccioStore.Core.Entities;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IPasswordRecoveryTokenRepository
    {
        Task<PasswordRecoveryToken> GetToken(string userEmail, CancellationToken cancellationToken);
        Task RemoveToken(CancellationToken cancellationToken);
        Task<bool> CreateAsync(CancellationToken cancellationToken, PasswordRecoveryToken token);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, PasswordRecoveryToken token);
    }
}
