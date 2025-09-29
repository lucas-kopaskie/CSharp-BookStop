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

public class UserService : IUserService
{
    private readonly BookStopContext _dataContext;
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;
    
    public UserService(BookStopContext dataContext, IConfiguration configuration, IAuthService authService)
    {
        _dataContext = dataContext;
        _configuration = configuration;
        _authService = authService;
    }
    
    public async Task<RegisterUserResultDto> RegisterUser(RegisterUserDto payload)
    {
        if (payload.Password.Equals(payload.ConfirmPassword).Equals(false))
        {
            return new RegisterUserResultDto(HttpStatusCode.BadRequest,
                "Passwords do not match.", null, null);
        }

        if (await _dataContext.Users.AnyAsync(u => u.Email == payload.Email))
        {
            return new RegisterUserResultDto(HttpStatusCode.Conflict, "Email already in use.", null, null);
        }
        
        var salt = RandomNumberGenerator.GetBytes(64);
        
        var hashedPassword = _authService.HashPassword(payload.Password, salt);

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
        
     _dataContext.Users.Add(user);
     _dataContext.UserCarts.Add(userCart);
     await _dataContext.SaveChangesAsync();

     return new  RegisterUserResultDto(HttpStatusCode.Created, "User created.", user.UserId, user.Email);
    }

    public async Task<LoginUserResultDto> LoginUser(LoginUserDto payload)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                return new LoginUserResultDto(HttpStatusCode.BadRequest, "Invalid username/password.",
                    null, null);
            }
            
            var passwordMatch = _authService.VerifyPassword(payload.Password, Convert.FromHexString(user.PasswordSalt), user.PasswordHash);
            if (!passwordMatch)
            {
                return new LoginUserResultDto(HttpStatusCode.BadRequest, "Invalid username/password.",
                    null, null);
            }
            
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
            var key = _configuration["jwtSigningCredentials"]!;


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = "localhost",
                Audience = "localhost",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            
            return new LoginUserResultDto(HttpStatusCode.OK, "Login successful.", user.UserId, jwt);
    }
}