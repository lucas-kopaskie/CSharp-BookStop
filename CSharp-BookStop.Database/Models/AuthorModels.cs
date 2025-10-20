namespace CSharp_BookStop.Database.Models;

// Internal
public record GetAuthorsDto(Guid AuthorId, string AuthorName, string AuthorSlug);
public record ReferencedAuthorDto(Guid AuthorId, string AuthorName, string AuthorSlug);

// Request / Response
public record GetBooksByAuthorResponse(Guid AuthorId, string AuthorName, string Biography, DateOnly DateOfBirth, int BookCount,
    IEnumerable<ReferencedBookDto> Books);
public record GetAuthorsResponse(List<GetAuthorsDto> Authors, int Count);
public record GetAuthorResponse(Guid AuthorId, string AuthorName, string Biography,  DateOnly DateOfBirth, string Slug);
public record CreateAuthorRequest(string AuthorName, string Biography, DateOnly DateOfBirth);
public record UpdateAuthorRequest(Guid AuthorId, string AuthorName, string Biography, DateOnly DateOfBirth);