namespace newproj.Models.Base;

public abstract class EntityBaseClass
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}