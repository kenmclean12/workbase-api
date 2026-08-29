namespace newproj.Models.Base;

public abstract class EntityBaseClass
{
    public int Id { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}