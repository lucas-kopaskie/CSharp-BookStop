namespace CSharp_BookStop.Database.Models;

public record CreateGenreRequest(string GenreName);

public record GetGenresResponse(Guid GenreId, string GenreName);
public record GetGenreDto(Guid GenreId, string GenreName, IEnumerable<ReferencedBookDto> Books);
public record ReferencedGenreDto(Guid GenreId, string GenreName);