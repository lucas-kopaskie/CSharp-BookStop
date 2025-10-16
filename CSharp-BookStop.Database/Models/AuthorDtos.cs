namespace CSharp_BookStop.Database.Models;

// Internal
public record GetAuthorsDto(Guid AuthorId, string AuthorName);
public record ReferencedAuthorDto(Guid AuthorId, string AuthorName);

// Request / Response
public record GetAuthorResponse(Guid AuthorId, string AuthorName, string Biography, DateOnly DateOfBirth, 
    IEnumerable<ReferencedBookDto> Books);
public record GetAuthorsResponse(List<GetAuthorsDto> Authors, int Count);
public record CreateAuthorRequest(string AuthorName, string Biography, DateOnly DateOfBirth);
