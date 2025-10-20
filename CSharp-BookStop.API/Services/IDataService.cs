namespace CSharp_BookStop.API.Services;

public interface IDataService
{
    public string GenerateSlug(Guid id, string identifier);
}