using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace CatenaccioStore.Infrastructure.Repositories.Implementation
{
    public class UserConfirmationRepository : IUserConfirmationRepository
    {
        private readonly IBaseRepo<UserConfirmationToken> _repository;
        public UserConfirmationRepository(IBaseRepo<UserConfirmationToken> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
        }
        public async Task<bool> CreateAsync(CancellationToken cancellationToken, UserConfirmationToken token)
        {
            await _repository.AddAsync(token, cancellationToken);
            return await _repository.SaveChangesAsync(cancellationToken);
        }


        public async Task<UserConfirmationToken> GetToken(string userEmail, CancellationToken cancellationToken)
        {
            return await _repository.Table.FirstOrDefaultAsync(i => i.UserEmail == userEmail);
        }
        public async Task<bool> DeleteAsync(CancellationToken cancellationToken, UserConfirmationToken token)
        {
            await _repository.RemoveAsync(cancellationToken, token.Id);
            return await _repository.SaveChangesAsync(cancellationToken);
        }
        public async Task RemoveToken(CancellationToken cancellationToken)
        {
            var tokens = await _repository.Table.ToListAsync();
            foreach (var t in tokens)
            {
                var dateTimeToCheck = t.ExpiryDate.AddHours(720);
                if (dateTimeToCheck < DateTime.Now)
                {
                    await _repository.RemoveAsync(cancellationToken, t.Id);
                    await _repository.SaveChangesAsync(cancellationToken);
                }
            }

        }
    }
}
