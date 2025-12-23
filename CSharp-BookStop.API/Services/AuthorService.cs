using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using UUIDNext;

namespace CSharp_BookStop.API.Services;

public class AuthorService(BookStopContext context, IDataService dataService) : IAuthorService
{
    public async Task<OneOf<AuthorDto, NotFound>> GetAuthorById(Guid id)
    {
        var author = await context.Authors.FindAsync(id);
        
        if (author == null)
        {
            return new NotFound();
        }
        return new AuthorDto(author.AuthorId, author.AuthorName, author.Biography, author.DateOfBirth, author.Slug);
    }

    public async Task<OneOf<BooksByAuthor, NotFound>> GetBooksByAuthorId(Guid id, int offset, int limit)
    {
        var authorBooks = await context.Authors
            .Include(a => a.Books)
            .ThenInclude(b => b.Genres)
            .Where(a => a.AuthorId == id)
            .Select(a => new BooksByAuthor(
                a.AuthorId, 
                a.Books
                    .OrderBy(b => b.BookId)
                    .Skip(offset)
                    .Take(limit)
                    .Select(b => new ReferencedBookDto(
                        b.BookId, 
                        b.Title, 
                        b.Summary, 
                        b.Authors
                            .Select(author =>
                                new ReferencedAuthorDto(author.AuthorId, author.AuthorName, author.Slug)).ToList(),
                        b.Slug))
                    .ToList(),
                a.Books.Count())
            )
            .SingleOrDefaultAsync();

        if (authorBooks == null)
        {
            return new NotFound();
        }
        return authorBooks;
    }

    public async Task<AuthorListDto> GetAuthors(int offset, int limit)
    {
        var authors = await context.Authors.OrderBy(a => a.AuthorName).Skip(offset).Take(limit)
            .Select(a => new AuthorListItemDto(a.AuthorId, a.AuthorName, a.Slug)).ToListAsync();
            
        var authorCount =  context.Authors.Count();

        return new AuthorListDto(authors, authorCount);
    }

    public async Task<OneOf<AuthorDto, Error<string>>> CreateAuthor(CreateAuthorRequest request)
    {
        var authorId = Uuid.NewDatabaseFriendly(UUIDNext.Database.PostgreSql);
        Author author = new()
        {
            AuthorId = authorId,
            AuthorName = request.AuthorName,
            Biography = request.Biography,
            DateOfBirth = request.DateOfBirth,
            Books = new List<Book>(),
            Slug = dataService.GenerateSlug(authorId, request.AuthorName),
        };

        try
        {
            context.Authors.Add(author);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return new Error<string>("An error occurred while trying to create a new author");
        }
        

        var authorResponse = new AuthorDto(author.AuthorId, author.AuthorName, author.Biography, author.DateOfBirth,
            author.Slug);
                
        return authorResponse;
    }

    public async Task<OneOf<Error<string>, NotFound, Success>> UpdateAuthor(UpdateAuthorRequest request)
    {
        var author = await context.Authors.FindAsync(request.AuthorId);

        if (author == null)
        {
            return new NotFound();
        }
            
        author.AuthorName = request.AuthorName;
        author.Biography = request.Biography;
        author.DateOfBirth = request.DateOfBirth;
        author.Slug = dataService.GenerateSlug(author.AuthorId, request.AuthorName);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return new Error<string>("An error occurred while updating your author");
        }

        return new Success();
    }

    public async Task<OneOf<NotFound, Success>> DeleteAuthor(Guid id)
    {
        var author = await context.Authors.FindAsync(id);
        if (author == null)
        {
            return new NotFound();
        }

        context.Authors.Remove(author);
        await context.SaveChangesAsync();

        return new Success();
    }
}