namespace CSharp_BookStop.API.Services;

public class DataService : IDataService
{
    public string GenerateSlug(Guid id, string identifier)
    {
        var frontEight = id.ToString()[..8];
        var normalizedIdentifier = identifier.ToLower();
        return normalizedIdentifier + "-" + frontEight;
    }
}