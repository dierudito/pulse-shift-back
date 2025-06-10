namespace dm.PulseShift.Infra.CrossCutting.Shared.Helpers;

public static class TimeZoneHelper
{
    public static TimeZoneInfo GetSaoPauloTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            }
            catch (TimeZoneNotFoundException)
            {
                // Se ambos falharem, pode-se lançar uma exceção customizada ou retornar um padrão,
                // dependendo do requisito. Para este exemplo, vamos relançar a exceção.
                // Em um cenário real, pode ser útil logar essa falha.
                throw new TimeZoneNotFoundException("Não foi possível encontrar o fuso horário de São Paulo em nenhum dos formatos conhecidos (Windows ou IANA).");
            }
            catch (InvalidTimeZoneException)
            {
                // Trata o caso de um ID de fuso horário IANA estar presente, mas os dados estarem corrompidos.
                throw new InvalidTimeZoneException("O fuso horário de São Paulo (IANA) foi encontrado, mas seus dados parecem estar corrompidos.");
            }
        }
        catch (InvalidTimeZoneException)
        {
            // Trata o caso de um ID de fuso horário do Windows estar presente, mas os dados estarem corrompidos.
            throw new InvalidTimeZoneException("O fuso horário de São Paulo (Windows) foi encontrado, mas seus dados parecem estar corrompidos.");
        }
    }
}
