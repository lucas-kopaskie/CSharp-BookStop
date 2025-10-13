using System.Net;

namespace CSharp_BookStop.Database.Models;

public record RegisterUserRequest(string Email, string Password, string ConfirmPassword);

public record LoginUserRequest(string Email, string Password);

public record GetUserResponse(Guid UserId, string Email);

public record TokenResponse(string Jwt);
public record RefreshTokenRequest(Guid UserId);

public interface IUserResponse
{
    HttpStatusCode Status { get; }
    string Message { get; }
}

public class LoginUserResponse(HttpStatusCode status, string message, string jwt, string refreshToken)
    : IUserResponse
{
    public HttpStatusCode Status { get; } = status;
    public string Message { get; } = message;
    public string Jwt { get; } = jwt;
    public string RefreshToken { get; } = refreshToken;
}

public class RegisterUserResponse(HttpStatusCode status, string message, Guid userId, string userEmail) : IUserResponse
{
    public HttpStatusCode Status { get; } = status;
    public string Message { get; } = message;
    public Guid UserId  { get; } = userId;
    public string UserEmail { get; } = userEmail;
}

public class UserResponseError(HttpStatusCode status, string message) : IUserResponse
{
    public HttpStatusCode Status { get; } = status;
    public string Message { get; } = message;
}