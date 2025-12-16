namespace CSharp_BookStop.Database.Models;

// Internal
public record GenreDto(Guid GenreId, string GenreName, IEnumerable<ReferencedGenreDto>? SubGenres);
public record ReferencedGenreDto(Guid GenreId, string GenreName);
public record GenreWithBooksDto(Guid GenreId, string GenreName, IEnumerable<ReferencedBookDto> Books, int Count);
public record GenreListDto(List<GenreListItemDto> Genres, int Count);
public record GenreListItemDto(Guid GenreId, string GenreName);

// Request / Response
public record CreateGenreRequest(string GenreName, Guid? ParentGenreId);
public record UpdateGenreRequest(Guid GenreId, string GenreName);