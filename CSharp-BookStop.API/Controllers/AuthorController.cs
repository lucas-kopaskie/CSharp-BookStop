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
    public class AuthorController(BookStopContext context) : ControllerBase
    {
        // GET: api/Author
        [HttpGet]
        public async Task<ActionResult<GetAuthorsResponse>> GetAuthors([FromQuery] int offset = 0, 
            [FromQuery] int limit = 10)
        {
            var authors = await context.Authors.OrderBy(a => a.AuthorName).Skip(offset).Take(limit)
                .Select(a => new GetAuthorsDto(a.AuthorId, a.AuthorName)).ToListAsync();
            
            var authorCount =  context.Authors.Count();

            return new GetAuthorsResponse(authors, authorCount);
        }

        // GET: api/Author/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GetAuthorResponse>> GetAuthor(Guid id, [FromQuery] int offset = 0, 
            [FromQuery] int limit = 10)
        {
            var author = await context.Authors.Where(a => a.AuthorId == id).Select(a =>
                new GetAuthorResponse(
                    a.AuthorId,
                    a.AuthorName,
                    a.Biography,
                    a.DateOfBirth,
                    a.Books.Select(b => new ReferencedBookDto(b.BookId, b.Title, b.Summary, 
                            b.Authors.Select(referencedAuthor => new ReferencedAuthorDto(referencedAuthor.AuthorId, referencedAuthor.AuthorName))))
                        .Skip(offset).Take(limit).ToList())
            ).SingleOrDefaultAsync();
            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // PUT: api/Author/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutAuthor(Guid id, Author author)
        {
            if (id != author.AuthorId)
            {
                return BadRequest();
            }

            context.Entry(author).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        // POST: api/Author
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GetAuthorResponse>> PostAuthor(CreateAuthorRequest payload)
        {
            Author author = new()
            {
                AuthorId = Guid.NewGuid(),
                AuthorName = payload.AuthorName,
                Biography = payload.Biography,
                DateOfBirth = payload.DateOfBirth,
                Books = new List<Book>()
            };
            
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.AuthorId }, author);
        }

        // DELETE: api/Author/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var author = await context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            context.Authors.Remove(author);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(Guid id)
        {
            return context.Authors.Any(e => e.AuthorId == id);
        }
    }
}
