using System.Xml.Serialization;

namespace Benner.Backend.Domain.Common;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    protected BaseEntity(Guid id) : this()
    {
        Id = id;
    }

    [XmlElement("Id")]
    public Guid Id { get; set; }

    [XmlElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [XmlElement("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [XmlElement("IsActive")]
    public bool IsActive { get; set; }

    public void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}