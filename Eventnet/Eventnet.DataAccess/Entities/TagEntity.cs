namespace Eventnet.DataAccess.Entities;

public class TagEntity
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    public int Id { get; private set; }
    public string Name { get; }
    public List<EventEntity> Events { get; set; } = new();

    public TagEntity(int id, string name)
    {
        Id = id;
        Name = name;
    }
}