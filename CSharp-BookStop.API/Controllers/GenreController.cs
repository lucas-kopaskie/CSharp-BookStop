using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController(BookStopContext context) : ControllerBase
    {
        // GET: api/Genre?limit=5;offset=0
        [HttpGet]
        public async Task<ActionResult<GetGenresResponse>> GetGenres([FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var genres = await context.Genres.OrderBy(g => g.GenreName).
                Skip(offset).Take(limit).Select(g => 
                new GetGenreDto(g.GenreId, g.GenreName)).ToListAsync();
            
            var count = context.Genres.Count();

            return new GetGenresResponse(genres, count);
        }
        
        // GET: api/Genre/Fantasy
        [HttpGet("{genreName}")]
        public async Task<ActionResult<GetGenreResponse>> GetGenreByName(string genreName)
        {
            var genre =  await context.Genres.Where(g => g.GenreName == genreName).Select(g => 
                new GetGenreResponse(g.GenreId, g.GenreName, g.SubGenres != null ? g.SubGenres.Select(
                    subgenre => new ReferencedGenreDto(subgenre.GenreId, subgenre.GenreName)).ToList() : null)).SingleOrDefaultAsync();
            
            if (genre == null)
            {
                return NotFound();
            }
            
            return genre;
        }

        // GET: api/Genre/Fantasy/books
        [HttpGet("{genreName:required}/books")]
        public async Task<ActionResult<GetBooksByGenreResponse>> GetBooksByGenre([FromRoute] string genreName, [FromQuery]  int offset = 0, [FromQuery] int limit = 10)
        {
            var genre = await context.Genres.Where(g => g.GenreName == genreName).SingleOrDefaultAsync();
            if (genre == null)
            {
                return NotFound();
            }
            
            var genreBooks = await context.Genres.Where(g => g.GenreName == genreName).Include(g => g.Books)
                .Select(g => new GetBooksByGenreResponse(g.GenreId, g.GenreName, context.Books.Count(book => book.Genres.Contains(genre)),
                    g.Books.Select(b => 
                        new ReferencedBookDto(b.BookId, b.Title, b.Summary, b.Authors.Select(a => 
                            new ReferencedAuthorDto(a.AuthorId, a.AuthorName, a.Slug)), b.Slug)).ToList())
                )
                .SingleOrDefaultAsync();

            if (genreBooks != null) return genreBooks;
            return NotFound();
        }

        // PUT: api/Genre/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutGenre(Guid id, UpdateGenreRequest request)
        {
            var genre = await context.Genres.FindAsync(id);
            if (genre is null || id != request.GenreId)
            {
                return BadRequest();
            }

            genre.GenreName = request.GenreName;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Genre
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GetGenreResponse>> PostGenre(CreateGenreRequest request)
        {
            var genre = await context.Genres.SingleOrDefaultAsync(g => g.GenreName == request.GenreName);
            if (genre != null)
            {
                return Conflict("Genre already exists.");
            }
            
            Genre newGenre = new()
            {
                GenreId = Guid.NewGuid(),
                GenreName = request.GenreName,
                ParentGenre = request.ParentGenreId is not null ?  await context.Genres.FirstOrDefaultAsync(g => g.GenreId == request.ParentGenreId) : null 
            };
            context.Genres.Add(newGenre);
            await context.SaveChangesAsync();
            
            var responseGenre = new GetGenreDto(newGenre.GenreId, newGenre.GenreName);

            return CreatedAtAction("GetGenreByName", new { genreName = newGenre.GenreName }, responseGenre);
        }

        // DELETE: api/Genre/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteGenre(Guid id)
        {
            var genre = await context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            context.Genres.Remove(genre);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool GenreExists(Guid id)
        {
            return context.Genres.Any(e => e.GenreId == id);
        }
    }
}
