namespace CSharp_BookStop.Database.Models;

// Internal
public record GetSeriesDto(Guid SeriesId, string Name, string Slug);
public record ReferencedSeriesDto(Guid SeriesId, string Name, string Slug);

// Request / Response
public record CreateSeriesRequest(string Name);
public record UpdateSeriesRequest(Guid SeriesId, string Name);
// Singular
public record GetSeriesResponse(Guid SeriesId, string Name, IEnumerable<ReferencedBookDto> Books);
// Plural
public record GetSeriesPluralResponse(List<GetSeriesDto> Series, int Count);