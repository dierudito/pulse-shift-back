using System.ComponentModel.DataAnnotations.Schema;

namespace dm.PulseShift.Domain.Entities.Insights;

[Table("activity_summary")]
public class ActivitySummary
{
    public string ActivityLabel { get; set; } = default!;
    public double Duration_Hours { get; set; }
}