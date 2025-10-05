using CSharp_BookStop.Database.Entities;

namespace CSharp_BookStop.API.Services;

public interface ITokenService
{
    public Task<TokenResponseDto?> RefreshTokens(RefreshTokenRequestDto payload);
    public Task<string> GenerateAndSaveRefreshTokenAsync(User user);
    public string CreateJwt(User user);
}