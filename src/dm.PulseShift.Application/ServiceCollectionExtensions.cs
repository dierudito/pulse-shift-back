﻿using dm.PulseShift.Application.AppServices;
using dm.PulseShift.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace dm.PulseShift.Application;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITimeEntryAppService, TimeEntryAppService>();
        //services.AddScoped<IAuthenticationWebSiteSeniorXAppService, AuthenticationWebSiteSeniorXAppService>();
        return services;
    }
}
