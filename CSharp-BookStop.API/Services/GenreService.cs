using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace CSharp_BookStop.API.Services;

public class GenreService(BookStopContext context) : IGenreService
{
    public async Task<OneOf<GenreDto, NotFound>> GetGenreByName(string genreName)
    {
        var genre =  await context.Genres.Where(g => g.GenreName == genreName).Select(g => 
            new GenreDto(g.GenreId, g.GenreName, g.SubGenres != null ? g.SubGenres.Select(
                    subgenre => new ReferencedGenreDto(subgenre.GenreId, subgenre.GenreName)).ToList()
                : null)).SingleOrDefaultAsync();
            
        if (genre == null)
        {
            return new NotFound();
        }
            
        return genre;
    }

    public async Task<OneOf<BooksByGenre, NotFound>> GetGenreWithBooks(string genreName, int offset, int limit)
    {
        var genreBooks = await context.Genres
            .Include(g => g.Books)
            .ThenInclude(b => b.Authors)
            .Where(g => g.GenreName == genreName)
            .Select(g =>
                new BooksByGenre(
                    g.GenreId,
                    g.Books
                        .OrderBy(b => b.BookId)
                        .Skip(offset)
                        .Take(limit)
                        .Select(b => new ReferencedBookDto(
                            b.BookId,
                            b.Title,
                            b.Summary,
                            b.Authors
                                .Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName, a.Slug))
                                .ToList(),
                            b.Slug))
                        .ToList(),
                    g.Books.Count()
                ))
            .SingleOrDefaultAsync();
            
        if (genreBooks == null)
        {
            return new NotFound();
        }
        return genreBooks;
    }

    public async Task<GenreListDto> GetGenres(int offset, int limit)
    {
        var genres = await context.Genres.OrderBy(g => g.GenreName).
            Skip(offset).Take(limit).Select(g => 
                new GenreListItemDto(g.GenreId, g.GenreName)).ToListAsync();
            
        var count = context.Genres.Count();

        return new GenreListDto(genres, count);
    }

    public async Task<OneOf<GenreDto, Error<string>>> CreateGenre(CreateGenreRequest request)
    {
        var genre = await context.Genres.SingleOrDefaultAsync(g => g.GenreName == request.GenreName);
        if (genre != null)
        {
            return new Error<string>("Genre already exists.");
        }
        
        Genre newGenre = new()
        {
            GenreId = Guid.NewGuid(),
            GenreName = request.GenreName,
            ParentGenre = request.ParentGenreId is not null
                ? await context.Genres.FirstOrDefaultAsync(g => g.GenreId == request.ParentGenreId)
                : null
        };
        context.Genres.Add(newGenre);
        await context.SaveChangesAsync();

        return new GenreDto(newGenre.GenreId, newGenre.GenreName, null);
    }

    public async Task<OneOf<Error<string>, NotFound, Success>> UpdateGenre(UpdateGenreRequest request)
    {
        var genre = await context.Genres.FindAsync(request.GenreId);
        if (genre is null)
        {
            return new NotFound();
        }

        genre.GenreName = request.GenreName;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return new Error<string>("There was an error updating your genre.");
        }

        return new Success();
    }

    public async Task<OneOf<NotFound, Success>> DeleteGenre(Guid id)
    {
        var genre = await context.Genres.FindAsync(id);
        if (genre == null)
        {
            return new NotFound();
        }

        context.Genres.Remove(genre);
        await context.SaveChangesAsync();

        return new Success();
    }
}