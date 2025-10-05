using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CSharp_BookStop.API.Services;

public class TokenService(IConfiguration configuration, BookStopContext dataContext) : ITokenService
{
    public async Task<TokenResponseDto?> RefreshTokens(RefreshTokenRequestDto payload)
    {
        var user = await ValidateRefreshTokenAsync(payload.UserId, payload.RefreshToken);
        if (user is null)
        {
            return null;
        }

        return new TokenResponseDto(CreateJwt(user), 
            await GenerateAndSaveRefreshTokenAsync(user));
    }
    
    public async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(14);
        await dataContext.SaveChangesAsync();
        return refreshToken;
    }

    public string CreateJwt(User user)
    {
        IEnumerable<Claim> claims =
        [
            new (JwtRegisteredClaimNames.Sub, user.UserId.ToString(), ClaimValueTypes.String),
            new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new (JwtRegisteredClaimNames.AuthTime, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new (JwtRegisteredClaimNames.Nonce, Guid.NewGuid().ToString(), ClaimValueTypes.String),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String),
            new (JwtRegisteredClaimNames.Email, user.Email, ClaimValueTypes.Email),
            new (ClaimTypes.Role, user.Role, ClaimValueTypes.String)
        ];
            
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = configuration["jwtSigningCredentials"]!;


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = "localhost",
            Audience = "localhost",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToHexString(bytes);
    }
    
    private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var user = await dataContext.Users.FindAsync(userId);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiration <= DateTime.Now)
        {
            return null;
        }

        return user;
    }
}