namespace CSharp_BookStop.API.Services;

public class DataService : IDataService
{
    public string GenerateSlug(Guid id, string identifier)
    {
        var frontSeven = id.ToString()[..7];
        var normalizedIdentifier = Uri.EscapeDataString(identifier).ToLower();
        return normalizedIdentifier + "-" + frontSeven;
    }
}