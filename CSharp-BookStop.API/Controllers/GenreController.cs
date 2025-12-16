using CSharp_BookStop.API.Services;
using CSharp_BookStop.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController(IGenreService genreService) : ControllerBase
    {
        // GET: api/Genre?limit=5;offset=0
        [HttpGet]
        public async Task<ActionResult<GenreListDto>> GetGenres([FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            return await genreService.GetGenres(offset, limit);
        }
        
        // GET: api/Genre/Fantasy
        [HttpGet("{genreName}")]
        public async Task<ActionResult<GenreDto>> GetGenreByName(string genreName)
        {
            var searchResult = await genreService.GetGenreByName(genreName);

            return searchResult.Match<ActionResult<GenreDto>>(
                dto => Ok(dto),
                notFound => NotFound());
        }

        // GET: api/Genre/Fantasy/books
        [HttpGet("{genreName:required}/books")]
        public async Task<ActionResult<GenreWithBooksDto>> GetBooksByGenre([FromRoute] string genreName,
            [FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            var genreWithBooks = await genreService.GetGenreWithBooks(genreName, offset, limit);
            
            return genreWithBooks.Match<ActionResult<GenreWithBooksDto>>(
                dto => Ok(dto),
                notFound => NotFound());
        }

        // PUT: api/Genre/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutGenre(Guid id, UpdateGenreRequest request)
        {
            var updatedGenre = await genreService.UpdateGenre(request, id);
            
            return updatedGenre.Match<IActionResult>(
                error => BadRequest(error),
                notFound => NotFound(),
                success => NoContent());
        }

        // POST: api/Genre
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GenreDto>> PostGenre(CreateGenreRequest request)
        {
            var createdGenre = await genreService.CreateGenre(request);
            
            return createdGenre.Match<ActionResult<GenreDto>>(
                dto => Ok(dto),
                error => StatusCode(500, error));
        }

        // DELETE: api/Genre/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteGenre(Guid id)
        {
            var deletedGenre = await genreService.DeleteGenre(id);
            
            return deletedGenre.Match<IActionResult>(
                notFound => NotFound(),
                success => NoContent());
        }
    }
}
