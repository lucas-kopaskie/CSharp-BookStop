namespace CSharp_BookStop.Database.Entities;

public record GetBookDto(Guid BookId, string Title, string Summary, decimal Price, DateOnly PublishDate, IEnumerable<ReferencedGenreDto> Genres, 
    IEnumerable<ReferencedAuthorDto> Authors);

public record GetBooksDto(Guid BookId, string Title, string Summary, DateOnly PublishDate, IEnumerable<ReferencedAuthorDto> Authors);

public record CreateBookDto(string Title, string Summary, DateOnly PublishDate, decimal Price, 
    ICollection<Guid> Genres, ICollection<Guid> Authors);

public record ReferencedBookDto(Guid BookId, string Title, string Summary);