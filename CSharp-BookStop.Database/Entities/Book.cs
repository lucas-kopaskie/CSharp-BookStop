using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.Database.Entities;

[Index(nameof(Title), nameof(BookId))]
[Index(nameof(Slug), IsUnique = true)]
public class Book
{
    public Guid BookId { get; set; }
    [MaxLength(64)]
    public required string Title { get; set; }
    [MaxLength(2048)]
    public required string Summary { get; set; }
    public required DateOnly PublishDate { get; set; }
    [Precision(18,2)]
    public required decimal Price { get; set; }
    public Series? Series { get; set; }
    public int? SeriesNumber { get; set; }
    public required ICollection<Genre> Genres { get; set; }
    public required ICollection<Author> Authors { get; set; }
    [MaxLength(128)]
    public required string Slug { get; set; }
}