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

// DI para servicos da API
builder.Services.AddSingleton<IAppConfigProvider, JsonAppConfigProvider>();
builder.Services.AddSingleton<IDirectoryDeployer, FileSystemDirectoryDeployer>();
builder.Services.AddSingleton<IDeploymentValidator, DeploymentValidator>();
builder.Services.AddSingleton<ISiteController, IisSiteController>();
builder.Services.AddSingleton<DeploymentService>();

// MVC Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddSwaggerConfig();

var app = builder.Build();

if (string.IsNullOrWhiteSpace(apiOptions.ApiKey))
{
    throw new InvalidOperationException("Api:ApiKey não configurada. Defina uma chave forte antes de iniciar a API.");
}

// Swagger + Controllers
app.UseSwaggerConfig();
app.MapControllers();

app.Run(apiOptions.ListenUrl);