namespace CSharp_BookStop.Database.Entities;

public record CreateGenreRequest(string GenreName);

public record GetGenresDto(Guid GenreId, string GenreName);
public record GetGenreDto(Guid GenreId, string GenreName, IEnumerable<ReferencedBookDto> Books);
public record ReferencedGenreDto(Guid GenreId, string GenreName);