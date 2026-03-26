using ChatIllinoisHelper.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Add user secrets in development
if (builder.Environment.IsDevelopment()) {
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddScoped(provider => {
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new ChatbotInterface(configuration["ApiUrl"], configuration["Workspace"], configuration["ApiKey"]);
});

builder.Build().Run();
