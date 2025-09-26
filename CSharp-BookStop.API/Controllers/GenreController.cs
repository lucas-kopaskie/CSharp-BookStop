using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Models;
using Microsoft.AspNetCore.Authorization;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly BookStopContext _context;

        public GenreController(BookStopContext context)
        {
            _context = context;
        }

        // GET: api/Genre
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            return await _context.Genres.ToListAsync();
        }

        // GET: api/Genre/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Genre>> GetGenre(Guid id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            return genre;
        }

        // PUT: api/Genre/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutGenre(Guid id, Genre genre)
        {
            if (id != genre.GenreId)
            {
                return BadRequest();
            }

            _context.Entry(genre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Genre>> PostGenre(CreateGenreDto payload)
        {
            Genre genre = new()
            {
                GenreId = Guid.NewGuid(),
                GenreName = payload.genreName
            };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGenre", new { id = genre.GenreId }, genre);
        }

        // DELETE: api/Genre/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteGenre(Guid id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GenreExists(Guid id)
        {
            return _context.Genres.Any(e => e.GenreId == id);
        }
    }
}
