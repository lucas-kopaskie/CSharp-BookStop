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
    public class BookController(BookStopContext context, DataService dataService) : ControllerBase
    {
        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<GetBooksResponse>> GetBooks([FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var books = await context.Books.OrderBy(b => b.Title).ThenBy(b => b.BookId).
                Skip(offset).Take(limit).Select(b => new GetBooksDto(b.BookId, b.Title, b.Summary, b.PublishDate, 
                    b.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName, a.Slug)), b.Slug))
                .ToListAsync();

            var bookCount = context.Books.Count();
            
            return new GetBooksResponse(books, bookCount);
        }

        // GET: api/Book/bill%20bob-rqf380f
        [HttpGet("{slug:required}")]
        public async Task<ActionResult<GetBookResponse>> GetBook(string slug)
        {
            var book = await context.Books.Where(b => b.Slug == slug).Select(b =>
                new GetBookResponse(b.BookId,
                    b.Title,
                    b.Summary,
                    b.Price,
                    b.PublishDate,
                    b.Genres.Select(g => new ReferencedGenreDto(g.GenreId,
                        g.GenreName)
                    {

                    }),
                    b.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName, a.Slug)),
                    b.Slug)
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
        public async Task<IActionResult> PutBook([FromRoute] Guid id, [FromBody] UpdateBookRequest request)
        {
            
            var book = await context.Books.FindAsync(id);
            if (book == null || id != request.BookId)
            {
                return BadRequest();
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
        public async Task<ActionResult<GetBookResponse>> PostBook(CreateBookRequest request)
        {
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
            context.Books.Add(book);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.BookId }, book);
        }

        // DELETE: api/Book/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
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
