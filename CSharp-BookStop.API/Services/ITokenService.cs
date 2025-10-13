using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;

namespace CSharp_BookStop.API.Services;

public interface ITokenService
{
    public Task<TokenResponse?> RefreshTokens(Guid userId, string refreshTokenCookie);
    public Task<string> GenerateAndSaveRefreshTokenAsync(User user);
    public string CreateJwt(User user);
}