namespace Eventnet.DataAccess;

public class EventEntity
{
    public Guid Id
    {
        get; 
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local EF.Core autogenerate
        private set;
    }
    public string OwnerId { get; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public LocationEntity Location { get; set; } = new();

    private EventEntity(Guid id, 
        string ownerId, 
        DateTime startDate,
        DateTime? endDate, 
        string name,
        string description)
    {
        Id = id;
        OwnerId = ownerId;
        StartDate = startDate;
        EndDate = endDate;
        Name = name;
        Description = description.Trim();
    }
    
    public EventEntity(Guid id, 
        string ownerId, 
        DateTime startDate,
        DateTime? endDate, 
        string name,
        string description,
        LocationEntity location) : this(id, ownerId, startDate, endDate, name, description)
    {
        Location = location;
    }
}