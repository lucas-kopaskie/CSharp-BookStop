namespace CSharp_BookStop.Database.Models;

// Internal
public record SeriesDto(Guid SeriesId, string Name, string Slug);
public record BooksBySeries(Guid SeriesId, IEnumerable<ReferencedBookDto> Books, int BookCount);
public record SeriesListDto(List<SeriesListItemDto> Series, int SeriesCount);
public record SeriesListItemDto(Guid SeriesId, string Name, string Slug);

// Request / Response
public record CreateSeriesRequest(string Name);
public record UpdateSeriesRequest(Guid SeriesId, string Name);
