using AutoMapper;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Application.AutoMapper;
public class TimeEntryMap : Profile
{
    public TimeEntryMap()
    {
        CreateMap<DateTimeOffset, DateTime>().ConvertUsing(source => source.DateTime);
        CreateMap<TimeEntry, TimeEntryResponseViewModel>()
            .ForMember(viewModel => viewModel.EntryType, opt => opt.MapFrom(entity => entity.EntryType.ToString()))
            .ForMember(viewModel => viewModel.EntryDate, opt => opt.MapFrom(entity => entity.EntryDate.DateTime.AddHours(-3)))
            .ForMember(viewModel => viewModel.Description, opt => opt.MapFrom(entity => entity.Description));
    }
}