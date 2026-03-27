using App.Api;
using App.Api.Configurations;
using App.Application;
using App.Application.Ports;
using App.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Config (já vem automático, nem precisa recriar)
var apiOptions = builder.Configuration.GetSection("Api").Get<ApiOptions>() ?? new ApiOptions();

builder.Services.AddSingleton(apiOptions);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// DI para servicos da API
builder.Services.AddSingleton<IAppConfigProvider, JsonAppConfigProvider>();
builder.Services.AddSingleton<IDirectoryDeployer, FileSystemDirectoryDeployer>();
builder.Services.AddSingleton<IDeploymentValidator, DeploymentValidator>();
builder.Services.AddSingleton<ISiteController, IisSiteController>();
builder.Services.AddSingleton<DeploymentService>();
builder.Services.AddSingleton<IisMonitorService>();

builder.Services.AddTransient<ApiKeyMiddleware>();

// MVC Controllers
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Liberado",policy =>
    {
        policy.WithOrigins(
            "http://192.168.0.224:7077",
                "http://localhost:7077",
                "http://127.0.0.1:5055",
                "http://localhost:5055")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger
builder.Services.AddSwaggerConfig();

var app = builder.Build();

if (string.IsNullOrWhiteSpace(apiOptions.ApiKey))
{
    throw new InvalidOperationException("Api:ApiKey não configurada. Defina uma chave forte antes de iniciar a API.");
}

app.UseExceptionHandler();

app.UseCors("Liberado");

app.UseMiddleware<ApiKeyMiddleware>();

// Swagger + Controllers
app.UseSwaggerConfig();
app.MapControllers();

app.Run(apiOptions.ListenUrl);