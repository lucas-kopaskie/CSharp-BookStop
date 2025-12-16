using CSharp_BookStop.Database.Models;
using OneOf;
using OneOf.Types;

namespace CSharp_BookStop.API.Services;

public interface IAuthorService
{
    public Task<OneOf<GetAuthorDto, NotFound>> GetAuthorById(Guid id);
    public Task<OneOf<GetBooksByAuthorDto, NotFound>> GetBooksByAuthorId(Guid id, int offset, int limit);

    public Task<GetAuthorsDto> GetAuthors(int offset, int limit);
    
    public Task<OneOf<GetAuthorDto, Error<string>>> CreateAuthor(CreateAuthorRequest request);
    
    public Task<OneOf<Error<string>, NotFound, Success>> UpdateAuthor(UpdateAuthorRequest request, Guid id);
    public Task<OneOf<NotFound, Success>> DeleteAuthor(Guid id);
}