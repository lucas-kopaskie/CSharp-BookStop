using System.Collections.ObjectModel;
using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController(BookStopContext context) : ControllerBase
    {
        // GET: api/Series
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Series>>> GetSeries()
        {
            return await context.Series.ToListAsync();
        }

        // GET: api/Series/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Series>> GetSeries(Guid id)
        {
            var series = await context.Series.FindAsync(id);

            if (series == null)
            {
                return NotFound();
            }

            return series;
        }

        // PUT: api/Series/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutSeries(Guid id, Series series)
        {
            if (id != series.SeriesId)
            {
                return BadRequest();
            }

            context.Entry(series).State = EntityState.Modified;

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
        public async Task<ActionResult<Series>> PostSeries(CreateSeriesRequest payload)
        {
            Series series = new()
            {
                SeriesId = Guid.NewGuid(),
                Name = payload.Name,
                Books = new Collection<Book>()
            };
            context.Series.Add(series);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetSeries", new { id = series.SeriesId }, series);
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
