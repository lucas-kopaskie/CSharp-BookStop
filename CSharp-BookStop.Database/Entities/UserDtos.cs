using System.Net;

namespace CSharp_BookStop.Database.Entities;

public record RegisterUserDto(string Email, string Password, string ConfirmPassword);
public record RegisterUserResultDto(HttpStatusCode Status, string Message, Guid? UserId, string? UserEmail);

public record LoginUserDto(string Email, string Password);
public record LoginUserResultDto(HttpStatusCode Status, string Message, string? Jwt, string? RefreshToken);

public record GetUserDto(Guid UserId, string Email);

public record TokenResponseDto(string Jwt, string RefreshToken);
public record RefreshTokenRequestDto(Guid UserId, string RefreshToken);