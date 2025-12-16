using CSharp_BookStop.Database.Models;
using OneOf;
using OneOf.Types;

namespace CSharp_BookStop.API.Services;

public interface IBookService
{
    public Task<OneOf<BookDto, NotFound>> GetBookById(Guid id);
    public Task<OneOf<BookDto, NotFound>> GetBookBySlug(string slug);
    
    public Task<BookListDto> GetBooks(int offset, int limit);
    
    public Task<OneOf<BookDto, Error<string>>> CreateBook(CreateBookRequest createBookRequest);
    
    public Task<OneOf<Error<string>, NotFound, Success>> UpdateBook(UpdateBookRequest updateBookRequest, Guid id);
    public Task<OneOf<NotFound, Success>> DeleteBook(Guid id);
    
}