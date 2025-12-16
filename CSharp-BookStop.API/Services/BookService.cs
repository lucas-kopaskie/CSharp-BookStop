using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using UUIDNext;
using NotFound = OneOf.Types.NotFound;

namespace CSharp_BookStop.API.Services;

public class BookService(BookStopContext context, IDataService dataService) : IBookService
{

    public async Task<OneOf<GetBookDto, NotFound>> GetBookById(Guid id)
    {
        var book = await context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .Where(b => b.BookId == id).Select(b =>
                new GetBookDto(b.BookId,
                    b.Title,
                    b.Summary,
                    b.Price,
                    b.PublishDate,
                    b.Genres.Select(g => new ReferencedGenreDto(g.GenreId,
                        g.GenreName)),
                    b.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName, a.Slug)),
                    b.Slug)
            ).FirstOrDefaultAsync();

        if (book == null)
        {
            return new NotFound();
        };

        return book;
    }

    public async Task<OneOf<GetBookDto, NotFound>> GetBookBySlug(string slug)
    {
        var book = await context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .Where(b => b.Slug == slug).Select(b =>
                new GetBookDto(b.BookId,
                    b.Title,
                    b.Summary,
                    b.Price,
                    b.PublishDate,
                    b.Genres.Select(g => new ReferencedGenreDto(g.GenreId,
                        g.GenreName)),
                    b.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName, a.Slug)),
                    b.Slug)
            ).FirstOrDefaultAsync();

        if (book == null)
        {
            return new NotFound();
        }

        return book;
    }

    public async Task<GetBooksDto> GetBooks(int offset, int limit)
    {
        var books = await context.Books
            .Include(b => b.Authors)
            .OrderBy(b => b.Title)
            .ThenBy(b => b.BookId)
            .Skip(offset)
            .Take(limit)
            .Select(b => new BookListItemDto(
                b.BookId, 
                b.Title, 
                b.Summary, 
                b.PublishDate, 
                b.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName, a.Slug)).ToList(), 
                b.Slug))
            .ToListAsync();

        var bookCount = context.Books.Count();
            
        return new GetBooksDto(books, bookCount);
    }

    public async Task<OneOf<GetBookDto, Error<string>>> CreateBook(CreateBookRequest request)
    {
        // Ensure that all subgenres added to a book also have their parent genre added.
        var genres = await context.Genres.Where(g => request.Genres.Contains(g.GenreId)).Select(g => new
        {
            g.GenreId,
            g.ParentGenre
        }).ToListAsync();
        foreach (var genre in genres.ToList())
        {
            if (genre.ParentGenre is null || request.Genres.Contains(genre.ParentGenre.GenreId))
            {
                continue;
            }
                
            request.Genres.Add(genre.ParentGenre.GenreId);
        }
            
        var bookId = Uuid.NewDatabaseFriendly(UUIDNext.Database.PostgreSql);
        Book book = new()
        {
            BookId = bookId,
            Title = request.Title,
            Summary = request.Summary,
            PublishDate = request.PublishDate,
            Price = request.Price,
            Genres = context.Genres.Where(g => request.Genres.Contains(g.GenreId)).ToList(),
            Authors = context.Authors.Where(a => request.Authors.Contains(a.AuthorId)).ToList(),
            Slug = dataService.GenerateSlug(bookId, request.Title)
        };
        try
        {
            context.Books.Add(book);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return new Error<string>("An error occurred while adding your book.");
        }

            
        var bookResponse = new GetBookDto(book.BookId, book.Title, book.Summary, book.Price, book.PublishDate,
            book.Genres.Select(g => new ReferencedGenreDto(g.GenreId, g.GenreName)).ToList(), 
            book.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName, a.Slug)).ToList(), book.Slug);

        return bookResponse;
    }

    public async Task<OneOf<Error<string>, NotFound, Success>> UpdateBook(UpdateBookRequest request, Guid id)
    {
        var book = await context.Books.FindAsync(id);

        if (book == null)
        {
            return new NotFound();
        }

        if (request.BookId != book.BookId)
        {
            return new Error<string>("Book Id does not match");
        }
            
        book.Title = request.Title;
        book.Summary = request.Summary;
        book.Price = request.Price;
        book.PublishDate = request.PublishDate;
        book.Genres.Clear();
        book.Authors.Clear();
        var genres = await context.Genres.Where(g => request.Genres.Contains(g.GenreId)).ToListAsync();
        var authors =  await context.Authors.Where(a => request.Authors.Contains(a.AuthorId)).ToListAsync();
        book.Genres = genres;
        book.Authors = authors;
        book.Slug = dataService.GenerateSlug(book.BookId, request.Title);

        if (request.SeriesId is not null && request.SeriesNumber is not null)
        {
            var series = await context.Series.Where(s => s.SeriesId == request.SeriesId).SingleOrDefaultAsync();
            book.Series = series;
            book.SeriesNumber = request.SeriesNumber;
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(book.BookId))
            {
                return new NotFound();
            }
            return new Error<string>("An error occurred while updating your book.");
            
        }

        return new Success();
    }

    public async Task<OneOf<NotFound, Success>> DeleteBook(Guid id)
    {
        var book = await context.Books.FindAsync(id);
        if (book == null)
        {
            return new NotFound();
        }

        context.Books.Remove(book);
        await context.SaveChangesAsync();

        return new Success();
    }
    
    private bool BookExists(Guid id)
    {
        return context.Books.Any(e => e.BookId == id);
    }
}