using System.Collections.ObjectModel;
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
    public class SeriesController(BookStopContext context, DataService dataService) : ControllerBase
    {
        // GET: api/Series
        [HttpGet]
        public async Task<ActionResult<GetSeriesPluralResponse>> GetSeries([FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            var series = await context.Series.OrderBy(s => s.Name).ThenBy(s => s.SeriesId).
                Skip(offset).Take(limit).Select(s => new GetSeriesDto(s.SeriesId, s.Name, s.Slug)).ToListAsync();

            var seriesCount = context.Series.Count();

            return new GetSeriesPluralResponse(series, seriesCount);
        }

        // GET: api/Series/5
        [HttpGet("{slug:required}")]
        public async Task<ActionResult<GetSeriesResponse>> GetSeries(string slug)
        {
            var series = await context.Series.Where(s => s.Slug == slug).Select(s => new GetSeriesResponse(
                s.SeriesId,
                s.Name,
                s.Books.Select(b => new ReferencedBookDto(b.BookId, b.Title, b.Summary,
                    b.Authors.Select(a => new ReferencedAuthorDto(a.AuthorId, a.AuthorName, a.Slug)), b.Slug))))
                .SingleOrDefaultAsync();

            if (series == null)
            {
                return NotFound();
            }

            return series;
        }

        // PUT: api/Series/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutSeries([FromRoute] Guid id, [FromBody] UpdateSeriesRequest request)
        {
            var series = await context.Series.FindAsync(id);
            if (series == null || id != request.SeriesId)
            {
                return BadRequest();
            }
            
            series.Name = request.Name;
            series.Slug = dataService.GenerateSlug(id, request.Name);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeriesExists(id))
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

        // POST: api/Series
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Series>> PostSeries(CreateSeriesRequest request)
        {
            var seriesId = Uuid.NewDatabaseFriendly(UUIDNext.Database.PostgreSql);
            Series series = new()
            {
                SeriesId = seriesId,
                Name = request.Name,
                Books = new Collection<Book>(),
                Slug = dataService.GenerateSlug(seriesId, request.Name)
            };
            context.Series.Add(series);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetSeries", new { slug = series.Slug }, series);
        }

        // DELETE: api/Series/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSeries(Guid id)
        {
            var series = await context.Series.FindAsync(id);
            if (series == null)
            {
                return NotFound();
            }

            context.Series.Remove(series);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool SeriesExists(Guid id)
        {
            return context.Series.Any(e => e.SeriesId == id);
        }
    }
}
