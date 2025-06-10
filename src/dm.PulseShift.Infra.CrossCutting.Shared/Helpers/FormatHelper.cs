using System.Globalization;

namespace dm.PulseShift.Infra.CrossCutting.Shared.Helpers;

public static class FormatHelper
{
    public static readonly CultureInfo PtBrCulture = CultureInfo.GetCultureInfo("pt-BR");
    public const string BrazilianDateTimeFormat = "dd/MM/yyyy HH:mm";

    public static string FormatNumberToBrazilianString(decimal number) => number.ToString("F2", PtBrCulture);
    public static string FormatDateTimeOffsetToBrazilianString(DateTimeOffset dateTimeOffset, string format = BrazilianDateTimeFormat)
    {
        DateTime saoPauloTime = TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneHelper.GetSaoPauloTimeZone()).DateTime;
        return saoPauloTime.ToString(format, PtBrCulture);
    }

    public static string? FormatDateTimeOffsetToBrazilianString(DateTimeOffset? dateTimeOffset, string format = BrazilianDateTimeFormat)
    {
        if (!dateTimeOffset.HasValue)
        {
            return null;
        }
        return FormatDateTimeOffsetToBrazilianString(dateTimeOffset.Value, format);
    }
}