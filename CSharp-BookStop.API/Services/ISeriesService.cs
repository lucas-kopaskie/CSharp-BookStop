using CSharp_BookStop.Database.Models;
using OneOf;
using OneOf.Types;

namespace CSharp_BookStop.API.Services;

public interface ISeriesService
{
    public Task<OneOf<SeriesDto, NotFound>> GetSeriesById(Guid seriesId);
    public Task<OneOf<SeriesWithBooksDto, NotFound>> GetBooksBySeries(Guid seriesId, int offset, int limit);
    
    public Task<SeriesListDto> GetSeries(int offset, int limit);
    
    public Task<OneOf<SeriesDto, Error<string>>> CreateSeries(CreateSeriesRequest request);
    
    public Task<OneOf<Error<string>, NotFound, Success>> UpdateSeries(UpdateSeriesRequest request);
    
    public Task<OneOf<NotFound, Success>> DeleteSeries(Guid id);
}