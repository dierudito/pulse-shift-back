using AutoMapper;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Application.AutoMapper;

public class ActivityPeriodMap : Profile
{
    public ActivityPeriodMap()
    {
        CreateMap<ActivityPeriod, ActivityPeriodResponseViewModel>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.LocalDateTime))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.LocalDateTime : (DateTimeOffset?)null))
            .ForMember(dest => dest.StartAssociatedTimeEntryId, opt => opt.MapFrom(src => src.AssociatedStartTimeEntryId))
            .ForMember(dest => dest.EndAssociatedTimeEntryId, opt => opt.MapFrom(src => src.AssociatedEndTimeEntryId));
    }
}