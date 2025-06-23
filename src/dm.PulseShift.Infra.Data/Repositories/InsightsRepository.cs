using dm.PulseShift.Domain.Entities.Insights;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace dm.PulseShift.Infra.Data.Repositories;

public class InsightsRepository(InsightsDbContext dbContext) : IInsightsRepository
{
    public async Task<IEnumerable<ActivitySummary>> GetTopTimeConsumingActivitiesAsync(int count = 15)
    {
        return await dbContext.ActivitySummaries
            .AsNoTracking()
            .OrderByDescending(summary => summary.Duration_Hours)
            .Take(count)
            .ToListAsync();
    }
}