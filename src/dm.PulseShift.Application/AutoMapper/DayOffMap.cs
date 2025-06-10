using AutoMapper;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Application.AutoMapper;

public class DayOffMap : Profile
{
    public DayOffMap()
    {
        CreateMap<DateTimeOffset, DateTime>().ConvertUsing(source => source.DateTime);
        CreateMap<CreateDayOffRequestViewModel, DayOff>();
    }
}