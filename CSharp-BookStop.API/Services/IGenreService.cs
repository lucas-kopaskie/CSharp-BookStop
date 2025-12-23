using CSharp_BookStop.Database.Models;
using OneOf;
using OneOf.Types;

namespace CSharp_BookStop.API.Services;

public interface IGenreService
{
    public Task<OneOf<GenreDto, NotFound>> GetGenreByName(string genreName);
    public Task<OneOf<BooksByGenre, NotFound>> GetGenreWithBooks(string genreName, int offset, int limit);
    
    public Task<GenreListDto> GetGenres(int offset, int limit);
    
    public Task<OneOf<GenreDto, Error<string>>> CreateGenre(CreateGenreRequest request);
    
    public Task<OneOf<Error<string>, NotFound, Success>> UpdateGenre(UpdateGenreRequest request);
    public Task<OneOf<NotFound, Success>> DeleteGenre(Guid id);
}