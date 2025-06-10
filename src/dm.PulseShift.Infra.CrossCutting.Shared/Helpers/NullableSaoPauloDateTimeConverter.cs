using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dm.PulseShift.Infra.CrossCutting.Shared.Helpers;

public class NullableSaoPauloDateTimeConverter : ValueConverter<DateTime?, DateTimeOffset?>
{
    private static readonly TimeZoneInfo SaoPauloZone = TimeZoneHelper.GetSaoPauloTimeZone();

    public NullableSaoPauloDateTimeConverter()
        : base(
            dateTime => dateTime.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Unspecified), SaoPauloZone.GetUtcOffset(dateTime.Value)) : null,
            dateTimeOffset => dateTimeOffset.HasValue ? TimeZoneInfo.ConvertTime(dateTimeOffset.Value, SaoPauloZone).DateTime : null)
    {
    }
}