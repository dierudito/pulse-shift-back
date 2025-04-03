using dm.PulseShift.bff.Endpoints;
using dm.PulseShift.bff.Extensions;
using dm.PulseShift.Infra.CrossCutting.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguration();
builder.Services.RegisterServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.ConfigureDevEnvironment();

app.UseCors(ApiConfigurations.CorsPolicyName);
app.MapEndpoints();

app.Run();
