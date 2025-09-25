using System.ComponentModel.DataAnnotations;

namespace CSharp_BookStop.API.Models;

public class Genre
{
    public required Guid GenreId { get; set; }
    [MaxLength(32)]
    public required string GenreName { get; set; }
    
    public IEnumerable<Book>? Books { get; set; }
}