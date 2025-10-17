namespace CSharp_BookStop.Database.Models;

// Internal
public record ReferencedGenreDto(Guid GenreId, string GenreName);

// Request / Response
public record CreateGenreRequest(string GenreName);
public record GetGenreResponse(Guid GenreId, string GenreName);
public record GetBooksByGenreResponse(Guid GenreId, string GenreName, int BookCount, IEnumerable<ReferencedBookDto> Books);