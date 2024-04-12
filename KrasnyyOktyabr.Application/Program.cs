WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
