using AutoMapper;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Application.AutoMapper;

public class DayOffMap : Profile
{
    public DayOffMap()
    {
        CreateMap<DateTime, DateTime>().ConvertUsing(source => source);
        CreateMap<CreateDayOffRequestViewModel, DayOff>();
    }
}