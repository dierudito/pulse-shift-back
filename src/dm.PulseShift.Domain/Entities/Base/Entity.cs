namespace dm.PulseShift.Domain.Entities.Base;
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.Now;
    }
}