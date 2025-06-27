using dm.PulseShift.Domain.Entities.Base;

namespace dm.PulseShift.Domain.Entities;

public class SprintReport : Entity
{
    public int NumeroSprint { get; set; }
    public string NomeSprint { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }

    public virtual ICollection<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
}

;
