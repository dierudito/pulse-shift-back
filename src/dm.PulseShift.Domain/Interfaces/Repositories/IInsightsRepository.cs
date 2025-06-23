using dm.PulseShift.Domain.Entities.Insights;

namespace dm.PulseShift.Domain.Interfaces.Repositories;

public interface IInsightsRepository
{
    Task<IEnumerable<ActivitySummary>> GetTopTimeConsumingActivitiesAsync(int count = 15);
    Task<IEnumerable<DailyProductivitySummary>> GetProductivityByDayOfWeekAsync();
}