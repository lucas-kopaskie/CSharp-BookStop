using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.Database.Entities;

[Index(nameof(AuthorName), nameof(AuthorId))]
[Index(nameof(Slug), IsUnique = true)]
public class Author
{
    public required Guid AuthorId {get; set;}
    [MaxLength(64)]
    public required string AuthorName {get; set;}
    [MaxLength(2048)]
    public required string Biography {get; set;}
    public required DateOnly DateOfBirth {get; set;}
    public required ICollection<Book> Books { get; set; } = new List<Book>();
    [MaxLength(128)]
    public required string Slug { get; set; }
}