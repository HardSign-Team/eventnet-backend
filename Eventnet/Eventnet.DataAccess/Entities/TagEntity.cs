namespace Eventnet.DataAccess.Entities;

public class TagEntity
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    public int Id { get; private set; }
    public string Name { get; }

    public TagEntity(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public TagEntity(string name) : this(0, name)
    {
    }
}