using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.Database.Entities;

[Index(nameof(GenreName), IsUnique = true)]
public class Genre
{
    public required Guid GenreId { get; set; }
    [MaxLength(32)]
    public required string GenreName { get; set; }

    public IEnumerable<Book> Books { get; set; } = [];
}