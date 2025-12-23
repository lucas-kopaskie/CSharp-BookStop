using CSharp_BookStop.API.Services;
using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController(ISeriesService seriesService) : ControllerBase
    {
        // GET: api/Series
        [HttpGet]
        public async Task<ActionResult<SeriesListDto>> GetSeries([FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            return await seriesService.GetSeries(offset, limit);
        }

        // GET: api/Series/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SeriesDto>> GetSeries(Guid id)
        {
            var searchResult = await seriesService.GetSeriesById(id);

            return searchResult.Match<ActionResult<SeriesDto>>(
                dto => Ok(dto),
                notFound => NotFound()
            );
        }
        
        // GET: api/Series/019a0869-7992-7380-8379-c819abaa00e7/books
        [HttpGet("{id:guid}/books")]
        public async Task<ActionResult<BooksBySeries>> GetBooksBySeries([FromRoute] Guid id, 
            [FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            var booksBySeries = await seriesService.GetBooksBySeries(id, offset, limit);

            return booksBySeries.Match<ActionResult<BooksBySeries>>(
                dto => Ok(dto),
                notFound => NotFound());
        }

        // PUT: api/Series/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutSeries([FromRoute] Guid id, [FromBody] UpdateSeriesRequest request)
        {
            var updatedSeries = await seriesService.UpdateSeries(request);

            return updatedSeries.Match<IActionResult>(
                error => BadRequest(error),
                notFound => NotFound(),
                success => NoContent());
        }

        // POST: api/Series
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Series>> PostSeries(CreateSeriesRequest request)
        {
            var  createdSeries = await seriesService.CreateSeries(request);

            return createdSeries.Match<ActionResult<Series>>(
                dto => CreatedAtAction("GetSeries", new { id = dto.SeriesId, dto }),
                error => StatusCode(500, error));
        }

        // DELETE: api/Series/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSeries(Guid id)
        {
            var deletedSeries = await seriesService.DeleteSeries(id);
            
            return deletedSeries.Match<IActionResult>(
                notFound => NotFound(),
                success => NoContent());
        }
    }
}
