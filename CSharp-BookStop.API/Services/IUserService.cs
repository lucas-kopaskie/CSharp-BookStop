using CSharp_BookStop.Database.Models;

namespace CSharp_BookStop.API.Services;

public interface IUserService
{
    public Task<IUserResponse> RegisterUser(RegisterUserRequest payload);
}