namespace dm.PulseShift.Domain.Dtos;

public record WorkTimeCalculationDto(TimeSpan TotalWorkHoursFromEntries, TimeSpan TotalWorkCoveredByActivities, TimeSpan UnaccountedWorkDuration);