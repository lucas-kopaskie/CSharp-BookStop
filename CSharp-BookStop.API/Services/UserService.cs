using System.Net;
using System.Security.Cryptography;
using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.API.Services;

public class UserService(BookStopContext dataContext, IAuthService authService)
    : IUserService
{
    public async Task<UserResponse> RegisterUser(RegisterUserRequest payload)
    {
        if (payload.Password.Equals(payload.ConfirmPassword).Equals(false))
        {
            return new UserResponseError(HttpStatusCode.BadRequest, "Passwords do not match");
        }

        if (await dataContext.Users.AnyAsync(u => u.Email == payload.Email))
        {
            return new UserResponseError(HttpStatusCode.Conflict, "Email already in use.");
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

     return new RegisterUserResponse(HttpStatusCode.Created, "User created.", user.UserId, user.Email);
    }
}