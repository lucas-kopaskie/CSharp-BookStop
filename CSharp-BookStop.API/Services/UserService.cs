using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CSharp_BookStop.API.Services;

public class UserService(BookStopContext dataContext, ITokenService tokenService, IAuthService authService)
    : IUserService
{
    public async Task<RegisterUserResultDto> RegisterUser(RegisterUserDto payload)
    {
        if (payload.Password.Equals(payload.ConfirmPassword).Equals(false))
        {
            return new RegisterUserResultDto(HttpStatusCode.BadRequest,
                "Passwords do not match.", null, null);
        }

        if (await dataContext.Users.AnyAsync(u => u.Email == payload.Email))
        {
            return new RegisterUserResultDto(HttpStatusCode.Conflict, "Email already in use.", null, null);
        }
        
        var salt = RandomNumberGenerator.GetBytes(64);
        
        var hashedPassword = authService.HashPassword(payload.Password, salt);

        User user = new()
        {
            UserId = Guid.NewGuid(),
            Email = payload.Email,
            PasswordSalt = Convert.ToHexString(salt),
            PasswordHash = hashedPassword,
            RefreshToken = "",
            Role = "User"
        };

        UserCart userCart = new()
        {
            UserId = user.UserId,
            UserCartId = Guid.NewGuid(),
        };
        
     dataContext.Users.Add(user);
     dataContext.UserCarts.Add(userCart);
     await dataContext.SaveChangesAsync();

     return new  RegisterUserResultDto(HttpStatusCode.Created, "User created.", user.UserId, user.Email);
    }
}