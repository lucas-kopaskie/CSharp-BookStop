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
    public class BookController(BookStopContext context) : ControllerBase
    {
        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBooksDto>>> GetBooks([FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            return await context.Books.OrderBy(b => b.Title).Skip(offset).Take(limit)
                .Select(b => new GetBooksDto(b.BookId, b.Title, b.Summary, b.PublishDate, 
                    b.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName)))).ToListAsync();
        }

        // GET: api/Book/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GetBookDto>> GetBook(Guid id)
        {
            var book = await context.Books.Where(b => b.BookId == id).Select(b =>
                new GetBookDto(b.BookId,
                    b.Title,
                    b.Summary,
                    b.Price,
                    b.PublishDate,
                    b.Genres.Select(g => new ReferencedGenreDto(g.GenreId,
                        g.GenreName)
                    {

                    }),
                    b.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName)))
            ).FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Book/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutBook(Guid id, Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            context.Entry(book).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Book
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GetBookDto>> PostBook(CreateBookRequest payload)
        {
            Book book = new()
            {
                BookId = Guid.NewGuid(),
                Title = payload.Title,
                Summary = payload.Summary,
                PublishDate = payload.PublishDate,
                Price = payload.Price,
                Genres = context.Genres.Where(g => payload.Genres.Contains(g.GenreId)).ToList(),
                Authors = context.Authors.Where(a => payload.Authors.Contains(a.AuthorId)).ToList(),
            };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.BookId }, book);
        }

        // DELETE: api/Book/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            context.Books.Remove(book);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(Guid id)
        {
            return context.Books.Any(e => e.BookId == id);
        }
    }
}
