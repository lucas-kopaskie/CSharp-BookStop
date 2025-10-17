using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.Database.Entities;

[Index(nameof(AuthorName), nameof(AuthorId))]
public class Author
{
    public required Guid AuthorId {get; set;}
    [MaxLength(32)]
    public required string AuthorName {get; set;}
    [MaxLength(1024)]
    public required string Biography {get; set;}
    public required DateOnly DateOfBirth {get; set;}

    public required ICollection<Book> Books { get; set; } = new List<Book>();
}