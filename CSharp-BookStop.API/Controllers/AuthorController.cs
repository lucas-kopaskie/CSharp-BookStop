using CSharp_BookStop.API.Services;
using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UUIDNext;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController(BookStopContext context, IDataService dataService) : ControllerBase
    {
        // GET: api/Author
        [HttpGet]
        public async Task<ActionResult<GetAuthorsResponse>> GetAuthors([FromQuery] int offset = 0, 
            [FromQuery] int limit = 10)
        {
            var authors = await context.Authors.OrderBy(a => a.AuthorName).Skip(offset).Take(limit)
                .Select(a => new GetAuthorsDto(a.AuthorId, a.AuthorName, a.Slug)).ToListAsync();
            
            var authorCount =  context.Authors.Count();

            return new GetAuthorsResponse(authors, authorCount);
        }

        // GET: api/Author/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GetAuthorResponse>> GetAuthor([FromRoute] Guid id)
        {
            var author = await context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return new GetAuthorResponse(author.AuthorId, author.AuthorName, author.Biography, author.DateOfBirth, author.Slug);
        }

        // GET: api/Author/019a0869-7992-7380-8379-c819abaa00e7/books
        [HttpGet("{id:guid}/books")]
        public async Task<ActionResult<GetBooksByAuthorResponse>> GetBooksByAuthor([FromRoute] Guid id, 
            [FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            var authorBooks = await context.Authors
                .Include(a => a.Books)
                .ThenInclude(b => b.Genres)
                .Where(a => a.AuthorId == id)
                .Select(a => new GetBooksByAuthorResponse(
                    a.AuthorId, 
                    a.AuthorName, 
                    a.Biography, 
                    a.DateOfBirth,
                    a.Books.Count(),
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
                        .ToList())
                )
                .SingleOrDefaultAsync();

            if (authorBooks == null)
            {
                return NotFound();
            }
            return authorBooks;
            
        }

        // PUT: api/Author/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutAuthor(Guid id, UpdateAuthorRequest request)
        {
            var author = await context.Authors.FindAsync(id);

            if (author == null || id != request.AuthorId)
            {
                return NotFound();
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
        public async Task<ActionResult<GetAuthorResponse>> PostAuthor(CreateAuthorRequest request)
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
