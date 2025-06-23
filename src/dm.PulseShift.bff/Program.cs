using dm.PulseShift.bff.Endpoints;
using dm.PulseShift.bff.Extensions;
using dm.PulseShift.Infra.CrossCutting.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguration();
builder.Services.RegisterServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ConfigureDevEnvironment();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Workforce Journey API v1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at apps root
    });
}
app.UseRouting();
app.UseCors(ApiConfigurations.CorsPolicyName);
app.MapEndpoints();

app.Run();
