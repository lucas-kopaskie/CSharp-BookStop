using System.ComponentModel.DataAnnotations;

namespace CSharp_BookStop.Database.Entities;

public class Book
{
    public Guid BookId { get; set; }
    [MaxLength(32)]
    public required string Title { get; set; }
    [MaxLength(1024)]
    public required string Summary { get; set; }
    public required DateOnly PublishDate { get; set; }
    public required decimal Price { get; set; }
    
    public required ICollection<Genre> Genres { get; set; }
    public required ICollection<Author> Authors { get; set; }
    
}