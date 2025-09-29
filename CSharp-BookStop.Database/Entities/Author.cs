using System.ComponentModel.DataAnnotations;

namespace CSharp_BookStop.Database.Entities;

public class Author
{
    public required Guid AuthorId {get; set;}
    [MaxLength(32)]
    public required string AuthorName {get; set;}
    [MaxLength(1024)]
    public required string Biography {get; set;}
    public required DateOnly DateOfBirth {get; set;}
    
    public IEnumerable<Book>? Books {get; set;}
}