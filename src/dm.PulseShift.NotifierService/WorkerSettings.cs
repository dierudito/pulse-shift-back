namespace dm.PulseShift.NotifierService;

public class WorkerSettings
{
    public string TargetWorkDuration { get; set; } = "08:00:00"; // Padrão seguro
    public int CheckIntervalMinutes { get; set; } = 15;        // Padrão seguro
}