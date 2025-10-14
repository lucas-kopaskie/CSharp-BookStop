using System.Net;

namespace CSharp_BookStop.Database.Models;

public record RegisterUserRequest(string Email, string Password, string ConfirmPassword);

public record LoginUserRequest(string Email, string Password);

public record GetUserResponse(Guid UserId, string Email);

public record TokenResponse(string Jwt);
public record RefreshTokenRequest(Guid UserId);

public record UserResponse(HttpStatusCode StatusCode, string Message);

public record LoginUserResponse(HttpStatusCode StatusCode, string Message, string Jwt, string RefreshToken)
    : UserResponse(StatusCode, Message);

public record RegisterUserResponse(HttpStatusCode StatusCode, string Message, Guid UserId, string Email) : UserResponse(StatusCode, Message);


public record UserResponseError(HttpStatusCode StatusCode, string Message) : UserResponse(StatusCode, Message);