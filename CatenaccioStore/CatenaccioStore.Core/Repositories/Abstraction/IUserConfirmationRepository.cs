using CatenaccioStore.Core.Entities;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IUserConfirmationRepository
    {
        Task<UserConfirmationToken> GetToken(string userEmail, CancellationToken cancellationToken);
        Task RemoveToken(CancellationToken cancellationToken);
        Task<bool> CreateAsync(CancellationToken cancellationToken, UserConfirmationToken token);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, UserConfirmationToken token);
    }
}
