using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.Database.Entities;

[Index(nameof(Name), nameof(SeriesId))]
[Index(nameof(Slug), IsUnique = true)]
public class Series
{
    public Guid SeriesId { get; set; }
    [MaxLength(64)]
    public required string Name { get; set; }
    public required ICollection<Book> Books { get; set; }
    [MaxLength(128)]
    public required string Slug { get; set; }
}