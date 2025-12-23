using System.Collections.ObjectModel;
using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using CSharp_BookStop.Database.Models;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using UUIDNext;

namespace CSharp_BookStop.API.Services;

public class SeriesService(BookStopContext context, IDataService dataService) : ISeriesService
{
    public async Task<OneOf<SeriesDto, NotFound>> GetSeriesById(Guid seriesId)
    {
        var series = await context.Series.Where(s => s.SeriesId == seriesId).Select(s => new SeriesDto(
                s.SeriesId,
                s.Name,
                s.Slug)).SingleOrDefaultAsync();

        if (series == null)
        {
            return new NotFound();
        }

        return series;
    }

    public async Task<OneOf<BooksBySeries, NotFound>> GetBooksBySeries(Guid seriesId, int offset, int limit)
    {
        var series = await context.Series.Where(s => s.SeriesId == seriesId).Select(s => 
                new BooksBySeries(
                s.SeriesId,
                s.Books.Select(b => new ReferencedBookDto(
                    b.BookId, 
                    b.Title, 
                    b.Summary,
                    b.Authors.Select(a => new ReferencedAuthorDto(
                        a.AuthorId, 
                        a.AuthorName, 
                        a.Slug))
                        .ToList(), 
                    b.Slug)).ToList(),
                s.Books.Count()))
            .SingleOrDefaultAsync();

        if (series == null)
        {
            return new NotFound();
        }

        return series;
    }

    public async Task<SeriesListDto> GetSeries(int offset, int limit)
    {
        var series = await context.Series.OrderBy(s => s.Name).ThenBy(s => s.SeriesId).
            Skip(offset).Take(limit).Select(s => new SeriesListItemDto(s.SeriesId, s.Name, s.Slug)).ToListAsync();

        var seriesCount = context.Series.Count();

        return new SeriesListDto(series, seriesCount);
    }

    public async Task<OneOf<SeriesDto, Error<string>>> CreateSeries(CreateSeriesRequest request)
    {
        var seriesId = Uuid.NewDatabaseFriendly(UUIDNext.Database.PostgreSql);
        Series series = new()
        {
            SeriesId = seriesId,
            Name = request.Name,
            Books = new Collection<Book>(),
            Slug = dataService.GenerateSlug(seriesId, request.Name)
        };

        try
        {
            context.Series.Add(series);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return new Error<string>("An error occurred while trying to create a new series");
        }

        return new SeriesDto(series.SeriesId, series.Name, series.Slug);
    }

    public async Task<OneOf<Error<string>, NotFound, Success>> UpdateSeries(UpdateSeriesRequest request)
    {
        var series = await context.Series.FindAsync(request.SeriesId);
        if (series == null)
        {
            return new NotFound();
        }
        
        series.Name = request.Name;
        series.Slug = dataService.GenerateSlug(request.SeriesId, request.Name);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return new Error<string>("There was an error updating your series.");
        }

        return new Success();
    }

    public async Task<OneOf<NotFound, Success>> DeleteSeries(Guid id)
    {
        var series = await context.Series.FindAsync(id);
        if (series == null)
        {
            return new NotFound();
        }

        context.Series.Remove(series);
        await context.SaveChangesAsync();

        return new Success();
    }
}