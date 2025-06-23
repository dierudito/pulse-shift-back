using System.ComponentModel.DataAnnotations.Schema;

namespace dm.PulseShift.Domain.Entities.Insights;

[Table("daily_productivity_summary")]
public class DailyProductivitySummary
{
    public string DayOfWeek { get; set; } = default!;
    public double Duration_Hours { get; set; }
}