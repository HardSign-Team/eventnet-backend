namespace Eventnet.DataAccess.Entities;

public class EventEntity
{
    public string Description { get; set; }
    public DateTime? EndDate { get; set; }

    public Guid Id
    {
        get;
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local EF.Core autogenerate
        private set;
    }

    public LocationEntity Location { get; set; } = new();
    public string Name { get; set; }
    public string OwnerId { get; }
    public DateTime StartDate { get; set; }
    public List<TagEntity> Tags { get; set; } = new();
    public List<SubscriptionEntity> Subscriptions { get; set; } = new();
    public List<MarkEntity> Marks { get; set; } = new();

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

    public SubscriptionEntity Subscribe(UserEntity user)
    {
        var subscription = new SubscriptionEntity(Id, user.Id, DateTime.Now);
        Subscriptions.Add(subscription);
        return subscription;
    }

    public void AddTag(TagEntity tagEntity)
    {
        Tags.Add(tagEntity);
    }

    public MarkEntity Like(UserEntity user) => new(user.Id, Id, true, DateTime.Now);

    public MarkEntity Dislike(UserEntity user) => new(user.Id, Id, false, DateTime.Now);
}