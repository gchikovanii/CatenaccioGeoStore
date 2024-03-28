using CatenaccioStore.Core.DTOs.Security;
using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Abstraction;
using Mapster;


namespace CatenaccioStore.Infrastructure.Services
{
    public class UserConfirmationService : IUserConfirmationService
    {
        private readonly IUserConfirmationRepository _repository;
        public UserConfirmationService(IUserConfirmationRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> CreateAsync(CancellationToken cancellationToken, TokenDto token)
        {
            var result = await _repository.CreateAsync(cancellationToken, token.Adapt<UserConfirmationToken>());
            return result;
        }

        public async Task<bool> DeleteAsync(CancellationToken cancellationToken, string userEmail)
        {
            var tokenExists = await _repository.GetToken(userEmail, cancellationToken);
            return await _repository.DeleteAsync(cancellationToken, tokenExists);
        }

        public async Task<TokenDto> GetToken(string userEmail, CancellationToken cancellationToken)
        {
            var reuslt = await _repository.GetToken(userEmail, cancellationToken);
            return reuslt.Adapt<TokenDto>();
        }
    }
}

