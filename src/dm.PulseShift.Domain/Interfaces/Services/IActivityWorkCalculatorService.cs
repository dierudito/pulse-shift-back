using dm.PulseShift.Domain.Dtos;
using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Domain.Interfaces.Services;

public interface IActivityWorkCalculatorService
{
    Task<TimeSpan> CalculateEffectiveWorkTimeAsync(Activity targetActivity);
    Task<WorkTimeCalculationDto> CalculateTotalEffectiveActivityTimeInRangeAsync(DateTime rangeStart, DateTime rangeEnd);
}