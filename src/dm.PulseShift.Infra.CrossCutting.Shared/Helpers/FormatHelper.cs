using System.Globalization;

namespace dm.PulseShift.Infra.CrossCutting.Shared.Helpers;

public static class FormatHelper
{
    public static readonly CultureInfo PtBrCulture = CultureInfo.GetCultureInfo("pt-BR");
    public const string BrazilianDateTimeFormat = "dd/MM/yyyy HH:mm";

    public static string FormatNumberToBrazilianString(double number) => number.ToString("F2", PtBrCulture);
    public static string FormatDateTimeToBrazilianString(DateTime date, string format = BrazilianDateTimeFormat)
    {
        return date.ToString(format, PtBrCulture);
    }

    public static string? FormatDateTimeToBrazilianString(DateTime? date, string format = BrazilianDateTimeFormat)
    {
        if (!date.HasValue)
        {
            return null;
        }
        return FormatDateTimeToBrazilianString(date.Value, format);
    }
    public static DateTime? ConvertPortugueseMonthDayToDateTime(this string dateStringInPortuguese)
    {
        const string format = "d 'de' MMMM";
        var cultureInfo = new CultureInfo("pt-BR");

        if (DateTime.TryParseExact(dateStringInPortuguese, format, cultureInfo, DateTimeStyles.None, out DateTime result))
        {
            return result;
        }

        return null;
    }
}