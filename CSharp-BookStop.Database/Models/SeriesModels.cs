namespace CSharp_BookStop.Database.Models;

// Internal
public record SeriesDto(Guid SeriesId, string Name, string Slug);
public record SeriesWithBooksDto(Guid SeriesId, string Name, IEnumerable<ReferencedBookDto> Books, string Slug);
public record SeriesListDto(List<SeriesListItemDto> Series, int Count);
public record SeriesListItemDto(Guid SeriesId, string Name, string Slug);

// Request / Response
public record CreateSeriesRequest(string Name);
public record UpdateSeriesRequest(Guid SeriesId, string Name);
