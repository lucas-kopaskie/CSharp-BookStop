using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.Database.Entities;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public Guid UserId { get; set; }
    [MaxLength(32)]
    public required string Email { get; set; }
    [MaxLength(128)]
    public required string PasswordSalt { get; set; }
    [MaxLength(128)]
    public required string PasswordHash { get; set; }
    [MaxLength(64)]
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
    [MaxLength(8)]
    public required string Role { get; set; } = "User";
    
    public UserCart? UserCart { get; set; }
}