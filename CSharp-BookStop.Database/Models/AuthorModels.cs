namespace CSharp_BookStop.Database.Models;

// Internal
public record AuthorDto(Guid AuthorId, string AuthorName, string Biography,  DateOnly DateOfBirth, string Slug);
public record ReferencedAuthorDto(Guid AuthorId, string AuthorName, string AuthorSlug);
public record AuthorListDto(List<AuthorListItemDto> Authors, int AuthorCount);
public record AuthorListItemDto(Guid AuthorId, string AuthorName, string AuthorSlug);
public record BooksByAuthor(Guid AuthorId, IEnumerable<ReferencedBookDto> Books, int BookCount);

// Request / Response
public record CreateAuthorRequest(string AuthorName, string Biography, DateOnly DateOfBirth);
public record UpdateAuthorRequest(Guid AuthorId, string AuthorName, string Biography, DateOnly DateOfBirth);