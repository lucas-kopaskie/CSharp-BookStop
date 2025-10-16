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
        public async Task<ActionResult<IEnumerable<GetGenresResponse>>> GetGenres([FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            return await context.Genres.OrderBy(g => g.GenreName).
                Skip(offset).Take(limit).Select(g => 
                new GetGenresResponse(g.GenreId, g.GenreName)).ToListAsync();
        }

        // GET: api/Genre/5?limit=5;offset=0
        [HttpGet("{genreName}")]
        public async Task<ActionResult<GetGenreResponse>> GetGenreAndBooks(string genreName, [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var genre = await context.Genres.Select(g => new
            GetGenreResponse(g.GenreId, g.GenreName, 
                g.Books.Select(b => new ReferencedBookDto(b.BookId, b.Title, b.Summary, 
                        b.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName))))
                .Skip(offset).Take(limit).ToList()){
            }).SingleOrDefaultAsync(g => g.GenreName == genreName);

            if (genre == null)
            {
                return NotFound();
            }

            return genre;
        }

        // PUT: api/Genre/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutGenre(Guid id, Genre genre)
        {
            if (id != genre.GenreId)
            {
                return BadRequest();
            }

            context.Entry(genre).State = EntityState.Modified;

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
        public async Task<ActionResult<GetGenreResponse>> PostGenre(CreateGenreRequest payload)
        {
            var genre = await context.Genres.SingleOrDefaultAsync(g => g.GenreName == payload.GenreName);
            if (genre != null)
            {
                return Conflict("Genre already exists.");
            }
            
            Genre newGenre = new()
            {
                GenreId = Guid.NewGuid(),
                GenreName = payload.GenreName
            };
            context.Genres.Add(newGenre);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetGenreAndBooks", new { id = newGenre.GenreId }, newGenre);
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
