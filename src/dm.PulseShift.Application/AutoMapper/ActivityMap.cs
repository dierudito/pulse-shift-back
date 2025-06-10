using AutoMapper;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Application.AutoMapper;

public class ActivityMap : Profile
{
    public ActivityMap()
    {
        CreateMap<Activity, ActivityResponseViewModel>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.LocalDateTime))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.HasValue ? src.UpdatedAt.Value.LocalDateTime : (DateTimeOffset?)null))
            .ForMember(dest => dest.Periods, opt => opt.MapFrom(src => src.ActivityPeriods))
            .ForMember(dest => dest.IsCurrentlyActive, opt => opt.MapFrom(src => src.GetCurrentOpenPeriod() != null));

        CreateMap<CreateActivityRequestViewModel, Activity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<Activity, ActivitySummaryResponseViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CardCode, opt => opt.MapFrom(src => src.CardCode))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}