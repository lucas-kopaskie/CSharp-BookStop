namespace CSharp_BookStop.Database.Entities;

public record CreateGenreDto(string GenreName);

public record GetGenresDto(Guid GenreId, string GenreName);
public record GetGenreDto(Guid GenreId, string GenreName, List<Book> Books);