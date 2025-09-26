using System.ComponentModel.DataAnnotations;

namespace CSharp_BookStop.Database.Models;

public class Book
{
    public Guid BookId { get; set; }
    [MaxLength(32)]
    public required string Title { get; set; }
    [MaxLength(1024)]
    public required string Summary { get; set; }
    public required DateOnly PublishDate { get; set; }
    public required decimal Price { get; set; }
    
    public required IEnumerable<Genre> Genres { get; set; }
    public required IEnumerable<Author> Authors { get; set; }
    
}