namespace CSharp_BookStop.Database.Models;

// Internal
public record AuthorListItemDto(Guid AuthorId, string AuthorName, string AuthorSlug);
public record ReferencedAuthorDto(Guid AuthorId, string AuthorName, string AuthorSlug);
public record GetAuthorDto(Guid AuthorId, string AuthorName, string Biography,  DateOnly DateOfBirth, string Slug);
public record GetAuthorsDto(List<AuthorListItemDto> Authors, int Count);
public record GetBooksByAuthorDto(Guid AuthorId, string AuthorName, string Biography, DateOnly DateOfBirth, int BookCount,
    IEnumerable<ReferencedBookDto> Books);

// Request / Response
public record CreateAuthorRequest(string AuthorName, string Biography, DateOnly DateOfBirth);
public record UpdateAuthorRequest(Guid AuthorId, string AuthorName, string Biography, DateOnly DateOfBirth);