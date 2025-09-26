namespace CSharp_BookStop.Database.Models;

public record RegisterUserDto(string Email, string Password, string ConfrimPassword);

public record LoginUserDto(string Email, string Password);

public record GetUserDto(Guid UserId, string Email);

public record GetAuthenticationTokenDto(Guid UserId, string TokenType, string Token);