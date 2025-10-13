namespace CSharp_BookStop.Database.Models;

public record GetAuthorsDto(Guid AuthorId, string AuthorName);
public record GetAuthorDto(Guid AuthorId, string AuthorName, string Biography, DateOnly DateOfBirth, 
    IEnumerable<ReferencedBookDto> Books);
public record CreateAuthorRequest(string AuthorName, string Biography, DateOnly DateOfBirth);
public record ReferencedAuthorDto(Guid AuthorId, string AuthorName);