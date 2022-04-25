// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local EFCORE

namespace Eventnet.DataAccess.Entities;

public class EventEntity
{
    public Guid Id { get; private set; }
    public string Description { get; set; }
    public DateTime? EndDate { get; set; }
    public LocationEntity Location { get; set; } = new();
    public string Name { get; set; }
    public string OwnerId { get; }
    public DateTime StartDate { get; set; }
    public List<TagEntity> Tags { get; set; } = new();
    public IReadOnlyCollection<SubscriptionEntity> Subscriptions { get; private set; } =
        new List<SubscriptionEntity>(0);
    public IReadOnlyCollection<MarkEntity> Marks { get; private set; } = new List<MarkEntity>(0);

    private EventEntity(
        Guid id,
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

    public EventEntity(
        Guid id,
        string ownerId,
        DateTime startDate,
        DateTime? endDate,
        string name,
        string description,
        LocationEntity location) : this(id, ownerId, startDate, endDate, name, description)
    {
        Location = location;
    }

    public SubscriptionEntity Subscribe(UserEntity user) => new(Id, user.Id, DateTime.Now);

    public void AddTag(TagEntity tagEntity)
    {
        Tags.Add(tagEntity);
    }
}