using System.ComponentModel.DataAnnotations;

namespace CSharp_BookStop.API.Models;

public class User
{
    public required int UserId { get; set; }
    [MaxLength(32)]
    public required string Username { get; set; }
    [MaxLength(32)]
    public required string Email { get; set; }
    [MaxLength(128)]
    public required string PasswordSalt { get; set; }
    [MaxLength(128)]
    public required string PasswordHash { get; set; }
    
    public required UserCart UserCart { get; set; }
}