using CatenaccioStore.Core.DTOs.Security;
using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Abstraction;
using Mapster;


namespace CatenaccioStore.Infrastructure.Services
{
    public class PasswordRecoveryTokenService : IPasswordRecoveryTokenService
    {
        private readonly IPasswordRecoveryTokenRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public PasswordRecoveryTokenService(IPasswordRecoveryTokenRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> CreateAsync(CancellationToken cancellationToken, TokenDto token)
        {
            var result = await _repository.CreateAsync(cancellationToken, token.Adapt<PasswordRecoveryToken>());
            return result;
        }

        public async Task<bool> DeleteAsync(CancellationToken cancellationToken, string userEmail)
        {
            var tokenExists = await _repository.GetToken(userEmail, cancellationToken);
            var result = await _repository.DeleteAsync(cancellationToken, tokenExists);
            return result;
        }

        public async Task<TokenDto> GetToken(string userEmail, CancellationToken cancellationToken)
        {
            var reuslt = await _repository.GetToken(userEmail, cancellationToken);
            return reuslt.Adapt<TokenDto>();
        }
    }
}
