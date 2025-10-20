namespace CSharp_BookStop.Database.Models;

// Internal
public record GetBooksDto(Guid BookId, string Title, string Summary, DateOnly PublishDate, IEnumerable<ReferencedAuthorDto> Authors, string Slug);
public record ReferencedBookDto(Guid BookId, string Title, string Summary, IEnumerable<ReferencedAuthorDto> Authors, string Slug);

// Request / Response
public record GetBookResponse(Guid BookId, string Title, string Summary, decimal Price, DateOnly PublishDate, IEnumerable<ReferencedGenreDto> Genres, 
    IEnumerable<ReferencedAuthorDto> Authors, string Slug);
public record CreateBookRequest(string Title, string Summary, DateOnly PublishDate, decimal Price, 
    ICollection<Guid> Genres, ICollection<Guid> Authors);
public record GetBooksResponse(List<GetBooksDto> Books, int Count);
public record UpdateBookRequest(Guid BookId, string Title, string Summary, DateOnly PublishDate, decimal Price, Guid? SeriesId, int? SeriesNumber,
    IEnumerable<Guid> Genres, IEnumerable<Guid> Authors);