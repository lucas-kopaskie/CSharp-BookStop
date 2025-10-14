using CSharp_BookStop.Database.Models;

namespace CSharp_BookStop.API.Services;

public interface IAuthService
{
    public string HashPassword(string password, byte[] salt);
    public Task<UserResponse> LoginUser(LoginUserRequest payload);
}