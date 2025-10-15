namespace CSharp_BookStop.Database.Models;

public record GetBookDto(Guid BookId, string Title, string Summary, decimal Price, DateOnly PublishDate, IEnumerable<ReferencedGenreDto> Genres, 
    IEnumerable<ReferencedAuthorDto> Authors);

public record GetBooksDto(Guid BookId, string Title, string Summary, DateOnly PublishDate, IEnumerable<ReferencedAuthorDto> Authors);
public record GetBooksResponse(List<GetBooksDto> Books, int Count);
public record CreateBookRequest(string Title, string Summary, DateOnly PublishDate, decimal Price, 
    ICollection<Guid> Genres, ICollection<Guid> Authors);

public record ReferencedBookDto(Guid BookId, string Title, string Summary, IEnumerable<ReferencedAuthorDto> Authors);