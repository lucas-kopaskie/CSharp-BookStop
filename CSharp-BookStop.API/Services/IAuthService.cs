using CSharp_BookStop.Database.Entities;

namespace CSharp_BookStop.API.Services;

public interface IAuthService
{
    public string HashPassword(string password, byte[] salt);
    public Task<LoginUserResponse> LoginUser(LoginUserRequest payload);
}