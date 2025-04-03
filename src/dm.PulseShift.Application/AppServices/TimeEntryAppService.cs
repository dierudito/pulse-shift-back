using AutoMapper;
using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;
using System.Net;

namespace dm.PulseShift.Application.AppServices;
public class TimeEntryAppService(ITimeEntryRepository repository, ITimeEntryService service, IMapper mapper) : ITimeEntryAppService
{
    public async Task<Response<TimeEntryResponseViewModel>> CreateAsync()
    {
        var currentDate = DateTimeOffset.UtcNow;
        var lastEntry = await repository.GetLastAsync();
        var entryType = TimeEntryType.ClockIn;

        if (lastEntry is not null && lastEntry.EntryDate.Date == currentDate.Date)
        {
            if(lastEntry.EntryType == TimeEntryType.ClockOut)
                return new ()
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = "You already clocked out today."
                };

            entryType = lastEntry.EntryType switch
            {
                TimeEntryType.ClockIn => TimeEntryType.BreakStart,
                TimeEntryType.BreakStart => TimeEntryType.BreakEnd,
                TimeEntryType.BreakEnd => TimeEntryType.ClockOut,
                _ => throw new ArgumentOutOfRangeException(nameof(lastEntry.EntryType), lastEntry.EntryType, null)
            };
        }

        var entity = new TimeEntry
        {
            EntryDate = currentDate,
            EntryType = entryType
        };

        var result = await service.AddAsync(entity);
        await repository.SaveChangesAsync();

        if (result is null)
            return new()
            {
                Code = HttpStatusCode.InternalServerError,
                Message = "An error occurred while creating the time entry."
            };
        var response = mapper.Map<TimeEntryResponseViewModel>(result);
        return new()
        {
            Code = HttpStatusCode.Created,
            Data = response
        };
    }

    public async Task<Response<GetTodaysDurationResponseViewModel>> GetTodaysDurationAsync()
    {
        var currentDate = DateTimeOffset.UtcNow;
        var entries = await repository.GetByDateAsync(currentDate.Date);
        if (entries is null || !entries.Any())
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(0,0,0, "00:00:00")
            };


    }
}
