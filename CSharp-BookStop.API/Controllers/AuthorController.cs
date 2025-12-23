using CSharp_BookStop.API.Services;
using CSharp_BookStop.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController(IAuthorService authorService) : ControllerBase
    {
        // GET: api/Author
        [HttpGet]
        public async Task<ActionResult<AuthorListDto>> GetAuthors([FromQuery] int offset = 0, 
            [FromQuery] int limit = 10)
        {
            return await authorService.GetAuthors(offset, limit);
        }

        // GET: api/Author/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor([FromRoute] Guid id)
        {
            var searchResult = await authorService.GetAuthorById(id);

            return searchResult.Match<ActionResult<AuthorDto>>(
                dto => Ok(dto),
                notFound => NotFound()
            );
        }

        // GET: api/Author/019a0869-7992-7380-8379-c819abaa00e7/books
        [HttpGet("{id:guid}/books")]
        public async Task<ActionResult<BooksByAuthor>> GetBooksByAuthor([FromRoute] Guid id, 
            [FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            var booksByAuthor = await authorService.GetBooksByAuthorId(id,  offset, limit);

            return booksByAuthor.Match<ActionResult<BooksByAuthor>>(
                dto => Ok(dto),
                notFound => NotFound());
        }

        // PUT: api/Author/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutAuthor(Guid id, UpdateAuthorRequest request)
        {
            var updatedAuthor = await authorService.UpdateAuthor(request);

            return updatedAuthor.Match<IActionResult>(
                error => BadRequest(error),
                notFound => NotFound(),
                success => NoContent());
        }

        // POST: api/Author
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> PostAuthor(CreateAuthorRequest request)
        {
            var createAuthorResult = await authorService.CreateAuthor(request);

            return createAuthorResult.Match<ActionResult<AuthorDto>>(
                dto => CreatedAtAction("GetAuthor", new { id = dto.AuthorId, dto }),
                error => StatusCode(500, error));
        }

        // DELETE: api/Author/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var deleteAuthorResult = await authorService.DeleteAuthor(id);

            return deleteAuthorResult.Match<IActionResult>(
                notFound => NotFound(),
                success => NoContent());
        }
    }
}
