using dm.PulseShift.Domain.Entities.Base;
using dm.PulseShift.Domain.Enums;

namespace dm.PulseShift.Domain.Entities;

public class WorkItem : Entity
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Responsavel { get; set; } = string.Empty;
    public WorkItemStatus Status { get; set; }
    public WorkItemType Tipo { get; set; }

    public Guid SprintReportId { get; set; }
    public virtual SprintReport SprintReport { get; set; } = default!;

    public Guid? ParentWorkItemId { get; set; }
    public virtual WorkItem? ParentWorkItem { get; set; }
    public virtual ICollection<WorkItem> SubWorkItems { get; set; } = [];
}