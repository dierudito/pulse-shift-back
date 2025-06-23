using AutoMapper;
using dm.PulseShift.Application.ViewModels.Responses.Charts;
using dm.PulseShift.Domain.Entities.Insights;

namespace dm.PulseShift.Application.AutoMapper;

public class DailyProductivitySummaryMap : Profile
{
    public DailyProductivitySummaryMap()
    {
        CreateMap<DailyProductivitySummary, ProductivityByDayViewModel>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
            .ForMember(dest => dest.DurationHours, opt => opt.MapFrom(src => src.Duration_Hours));
    }
}