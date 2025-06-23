using dm.PulseShift.Application.AppServices;
using dm.PulseShift.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace dm.PulseShift.Application;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITimeEntryAppService, TimeEntryAppService>();
        services.AddScoped<IDayOffAppService, DayOffAppService>();
        services.AddScoped<IActivityAppService, ActivityAppService>();
        services.AddScoped<IWorkSummaryAppService, WorkSummaryAppService>();
        services.AddScoped<IReportAppService, ReportAppService>();
        services.AddScoped<IChartReportAppService , ChartReportAppService>();
        //services.AddScoped<IAuthenticationWebSiteSeniorXAppService, AuthenticationWebSiteSeniorXAppService>();
        return services;
    }
}
