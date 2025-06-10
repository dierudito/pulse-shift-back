using dm.PulseShift.Domain.Interfaces.Services;
using dm.PulseShift.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace dm.PulseShift.Domain;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
        => services
            .AddScoped<ITimeEntryService, TimeEntryService>()
            .AddScoped<IDayOffService, DayOffService>()
            .AddScoped<IActivityService, ActivityService>()
            .AddScoped<IActivityWorkCalculatorService, ActivityWorkCalculatorService>()
        ;
}