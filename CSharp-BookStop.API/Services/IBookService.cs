using CSharp_BookStop.Database.Models;
using OneOf;
using OneOf.Types;

namespace CSharp_BookStop.API.Services;

public interface IBookService
{
    public Task<OneOf<GetBookDto, NotFound>> GetBookById(Guid id);
    public Task<OneOf<GetBookDto, NotFound>> GetBookBySlug(string slug);
    
    public Task<GetBooksDto> GetBooks(int offset, int limit);
    
    public Task<GetBookDto> CreateBook(CreateBookRequest createBookRequest);
    
    public Task<OneOf<Error, NotFound, Success>> UpdateBook(UpdateBookRequest updateBookRequest, Guid id);
    public Task<OneOf<NotFound, Success>> DeleteBook(Guid id);
    
}