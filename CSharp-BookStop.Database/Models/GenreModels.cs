namespace CSharp_BookStop.Database.Models;

// Internal
public record ReferencedGenreDto(Guid GenreId, string GenreName);

public record GetGenreDto(Guid GenreId, string GenreName);

// Request / Response
public record CreateGenreRequest(string GenreName, Guid? ParentGenreId);
public record GetGenresResponse(List<GetGenreDto> Genres, int Count);
public record GetGenreResponse(Guid GenreId, string GenreName, IEnumerable<ReferencedGenreDto>? Subgenres);
public record GetBooksByGenreResponse(Guid GenreId, string GenreName, int BookCount, IEnumerable<ReferencedBookDto> Books);
public record UpdateGenreRequest(Guid GenreId, string GenreName);