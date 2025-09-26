namespace CSharp_BookStop.Database.Models;

public record BookListingDto(Guid BookId, string Title, decimal Price, IEnumerable<Genre> Genres, 
    IEnumerable<Author> Authors);

public record CreateBookDto(string Title, string Summary, DateOnly PublishDate, decimal Price, 
    IEnumerable<Genre> Genres, IEnumerable<Author> Authors);

