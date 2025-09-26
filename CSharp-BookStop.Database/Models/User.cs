using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.Database.Models;

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
    
    public UserCart? UserCart { get; set; }
}