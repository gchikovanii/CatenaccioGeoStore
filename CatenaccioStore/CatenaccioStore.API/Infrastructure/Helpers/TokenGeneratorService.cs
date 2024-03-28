using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CatenaccioStore.API.Infrastructure.Helpers
{
    public class TokenGeneratorService
    {
        private readonly string _secretKey;
        private readonly double _expiryInMinutes;
        public TokenGeneratorService(string secretKey, double expiryInMinutes)
        {
            _secretKey = secretKey;
            _expiryInMinutes = expiryInMinutes;
        }
        public string GenerateToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Email, email)
            }),
                Expires = DateTime.UtcNow.AddHours(_expiryInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
