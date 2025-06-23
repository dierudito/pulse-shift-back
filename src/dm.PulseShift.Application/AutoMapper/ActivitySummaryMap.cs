using AutoMapper;
using dm.PulseShift.Application.ViewModels.Responses.Charts;
using dm.PulseShift.Domain.Entities.Insights;

namespace dm.PulseShift.Application.AutoMapper;

public class ActivitySummaryMap : Profile
{
    public ActivitySummaryMap()
    {
        CreateMap<ActivitySummary, TopActivityChartDataViewModel>()
            .ForMember(dest => dest.ActivityLabel, opt => opt.MapFrom(src => src.ActivityLabel))
            .ForMember(dest => dest.DurationHours, opt => opt.MapFrom(src => src.Duration_Hours));
    }
}