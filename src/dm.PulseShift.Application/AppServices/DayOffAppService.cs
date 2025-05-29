using AutoMapper;
using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;

namespace dm.PulseShift.Application.AppServices;

public class DayOffAppService(IDayOffRepository repository, IDayOffService service, IMapper mapper) :
    IDayOffAppService
{
    public async Task<Response<bool>> AddDayOffAsync(CreateDayOffRequestViewModel requestViewModel)
    {
        var entity = mapper.Map<DayOff>(requestViewModel);
        entity = await service.AddAsync(entity);
        await repository.SaveChangesAsync();
        return new()
        {
            Code = System.Net.HttpStatusCode.Created,
            Data = entity != null
        };
    }

    public async Task<long> CalcTimeUntilNextExecAsync(DateOnly date)
    {
        var twentyFourHourInSecond = (long)TimeSpan.FromHours(24).TotalSeconds;
        long timeUntilNextExec = 0;

        if (await IsDayOffAsync(date))
        {
            timeUntilNextExec += twentyFourHourInSecond;
            timeUntilNextExec += await CalcTimeUntilNextExecAsync(date.AddDays(1));
        }

        return timeUntilNextExec;
    }

    public async Task<bool> IsDayOffAsync(DateOnly date)
    {
        var entity = await repository.GetByDateAsync(date);
        return entity != null;
    }
}