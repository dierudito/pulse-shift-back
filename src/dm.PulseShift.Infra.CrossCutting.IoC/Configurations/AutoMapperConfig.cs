﻿using AutoMapper;
using dm.PulseShift.Application.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace dm.PulseShift.Infra.CrossCutting.IoC.Configurations;
public static class AutoMapperConfig
{
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        var mapConfig = new MapperConfiguration(mc =>
        {
            mc.AllowNullDestinationValues = true;
            mc.AllowNullCollections = true;

            mc.AddProfile(new TimeEntryMap());
            mc.AddProfile(new DayOffMap());
            mc.AddProfile(new ActivityMap());
            mc.AddProfile(new ActivityPeriodMap());
            mc.AddProfile(new ActivitySummaryMap());
            mc.AddProfile(new DailyProductivitySummaryMap());
        });

        var mapper = mapConfig.CreateMapper();
        return services.AddSingleton(mapper);
    }
}
