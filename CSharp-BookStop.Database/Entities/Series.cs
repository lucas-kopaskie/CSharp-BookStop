using System.ComponentModel.DataAnnotations;

namespace CSharp_BookStop.Database.Entities;

public class Series
{
    public Guid SeriesId { get; set; }
    [MaxLength(64)]
    public required string Name { get; set; }
    public required ICollection<Book> Books { get; set; }
}