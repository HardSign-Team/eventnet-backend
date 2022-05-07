using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Eventnet.Api.Models.Filtering;

public class EventsFilterModel
{
    public DateFilterModel? StartDate { get; init; }
    public DateFilterModel? EndDate { get; init; }
    public OwnerFilterModel? Owner { get; init; }
    public LocationFilterModel? RadiusLocation { get; init; }
    public TagsFilterModel? Tags { get; init; }
    
    public static bool TryParse(string base64Model, [NotNullWhen(true)] out EventsFilterModel? result)
    {
        try
        {
            var bytes = Convert.FromBase64String(base64Model);
            var json = Encoding.UTF8.GetString(bytes);
            result = JsonSerializer.Deserialize<EventsFilterModel>(json);
        }
        catch (Exception)
        {
            result = null;
        }

        return result != null;
    }
}