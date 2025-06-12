using dm.PulseShift.Infra.CrossCutting.Shared;

namespace dm.PulseShift.bff.Extensions;

public static class BuildExtension
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        ApiConfigurations.ConnectionString =
            builder.Configuration.GetConnectionString("PulseShiftDbSqlServer") ?? string.Empty;
        ApiConfigurations.BackendUrl =
            builder.Configuration.GetValue<string>("Config:Cors:BackendUrl") ?? string.Empty;
        ApiConfigurations.FrontendUrl =
            builder.Configuration.GetValue<string>("Config:Cors:FrontendUrl") ?? string.Empty;
        ApiConfigurations.CorsPolicyName =
            builder.Configuration.GetValue<string>("Config:Cors:Name") ?? string.Empty;
    }
}