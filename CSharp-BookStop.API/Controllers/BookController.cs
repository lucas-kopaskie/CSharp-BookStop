using CSharp_BookStop.API.Services;
using CSharp_BookStop.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(IBookService bookService) : ControllerBase
    {
        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<GetBooksDto>> GetBooks([FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            return await bookService.GetBooks(offset, limit);
        }

        // GET: api/Book/bill%20bob-rqf380f
        [HttpGet("bySlug/{slug:required}")]
        public async Task<ActionResult<GetBookDto>> GetBookBySlug(string slug)
        {
            var searchResult = await bookService.GetBookBySlug(slug);

            return searchResult.Match<ActionResult<GetBookDto>>(
                dto => Ok(dto),
                notFound => NotFound());
        }
        
        // GET: api/Book/{guid}
        [HttpGet("byId/{id:guid}")]
        public async Task<ActionResult<GetBookDto>> GetBookById(Guid id)
        {
            var  searchResult = await bookService.GetBookById(id);
            
            return searchResult.Match<ActionResult<GetBookDto>>(
                dto => Ok(dto),
                notFound => NotFound());
        }

        // PUT: api/Book/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutBook([FromRoute] Guid id, [FromBody] UpdateBookRequest request)
        {
            var updateBookResult =  await bookService.UpdateBook(request, id);
            
            return updateBookResult.Match<IActionResult>(
                error => BadRequest(),
                notFound => NotFound(),
                success => NoContent());
        }

        // POST: api/Book
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GetBookDto>> PostBook(CreateBookRequest request)
        {
            var createBookResult = await bookService.CreateBook(request);
            
            return CreatedAtAction("GetBookById", new { id = createBookResult.BookId }, createBookResult);
        }

        // DELETE: api/Book/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var deleteBookResult = await bookService.DeleteBook(id);

            return deleteBookResult.Match<IActionResult>(
                notFound => NotFound(),
                success => NoContent());
        }
    }
}
