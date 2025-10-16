using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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

        const string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";
        if (!Regex.IsMatch(payload.Password, pattern))
        {
            return new UserResponseError(HttpStatusCode.BadRequest, "Passwords must contain at least one uppercase," +
                                                                    " one lowercase, one number and one special character.");
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

        Cart userCart = new()
        {
            UserId = user.UserId,
            CartId = Guid.NewGuid(),
        };
        
     dataContext.Users.Add(user);
     dataContext.Carts.Add(userCart);
     await dataContext.SaveChangesAsync();

     return new RegisterUserResponse(HttpStatusCode.Created, "User created.", user.UserId, user.Email);
    }
}