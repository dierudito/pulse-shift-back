namespace dm.PulseShift.Application.Interfaces;

public interface IWorkerAppService
{
    Task<TimeSpan> GetTodaysDurationAsync();
}