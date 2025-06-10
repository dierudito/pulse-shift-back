using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dm.PulseShift.Infra.CrossCutting.Shared.Helpers;

public class SaoPauloDateTimeConverter : ValueConverter<DateTime, DateTimeOffset>
{
    private static readonly TimeZoneInfo SaoPauloZone = TimeZoneHelper.GetSaoPauloTimeZone();

    public SaoPauloDateTimeConverter()
        : base(
            dateTime => new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), SaoPauloZone.GetUtcOffset(dateTime)),
            dateTimeOffset => TimeZoneInfo.ConvertTime(dateTimeOffset, SaoPauloZone).DateTime)
    {
    }
}
