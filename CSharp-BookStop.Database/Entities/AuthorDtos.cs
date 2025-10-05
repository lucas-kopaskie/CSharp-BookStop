namespace CSharp_BookStop.Database.Entities;

public record GetAuthorsDto(Guid AuthorId, string AuthorName);
public record GetAuthorDto(Guid AuthorId, string AuthorName, string Biography, DateOnly DateOfBirth, 
    IEnumerable<ReferencedBookDto> Books);
public record CreateAuthorDto(string AuthorName, string Biography, DateOnly DateOfBirth);
public record ReferencedAuthorDto(Guid AuthorId, string AuthorName);