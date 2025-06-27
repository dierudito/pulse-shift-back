using System.Runtime.Serialization;

namespace dm.PulseShift.Domain.Enums;

public enum WorkItemType
{
    Bug,
    Task,
    [EnumMember(Value = "Product Backlog Item")]
    ProductBacklogItem
}