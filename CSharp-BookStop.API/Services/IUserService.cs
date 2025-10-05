using System.Net;
using CSharp_BookStop.Database.Entities;

namespace CSharp_BookStop.API.Services;

public interface IUserService
{
    public Task<RegisterUserResultDto> RegisterUser(RegisterUserDto payload);
}