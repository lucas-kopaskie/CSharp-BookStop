using System.Net;

namespace CSharp_BookStop.Database.Entities;

public record RegisterUserRequest(string Email, string Password, string ConfirmPassword);
public record RegisterUserResponse(HttpStatusCode Status, string Message, Guid? UserId, string? UserEmail);

public record LoginUserRequest(string Email, string Password);
public record LoginUserResponse(HttpStatusCode Status, string Message, string? Jwt, string? RefreshToken);

public record GetUserRequest(Guid UserId, string Email);

public record TokenResponse(string Jwt);
public record RefreshTokenRequest(Guid UserId);