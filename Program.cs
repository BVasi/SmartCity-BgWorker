using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddSingleton<FileLoggerService>();
builder.Services.AddSingleton<ReportService>();
builder.ConfigureFunctionsWebApplication();
builder.Build().Run();
