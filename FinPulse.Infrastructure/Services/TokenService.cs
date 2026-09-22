using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace FinPulse.Services;

public class TokenService : ITokenService
{
    public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config)
    {
        var key = _config.GetSection("JWT:SecretKey").Value ??
            throw new InvalidOperationException("Chave JWT não configurada!");

        var privateKey = Encoding.UTF8.GetBytes(key);

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(privateKey),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JWT:TokenValidityInMinutes"] ?? "60")),
            Issuer = _config["JWT:ValidIssuer"],
            Audience = _config["JWT:ValidAudience"],
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return token;
    }

    public string GenerateRefreshToken()
    {
        var secureRandomBytes = new byte[128];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(secureRandomBytes);

        return Convert.ToBase64String(secureRandomBytes);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config)
    {
        var secretKey = _config["JWT:SecretKey"] ??
            throw new InvalidOperationException("Chave secreta não encontrada.");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false // Importante: aceita token expirado para ler as claims e renovar com Refresh Token
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Token inválido.");
        }

        return principal;
    }
}