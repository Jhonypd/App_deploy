using App_deploy.Application;
using App_deploy.Application.Ports;
using App_deploy.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace App_deploy.Api;

public static class ApiServer
{
	public static async Task RunAsync(string? listenUrl)
	{
		var builder = WebApplication.CreateBuilder();

		builder.Configuration
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

		builder.Services.AddSingleton<IAppConfigProvider, JsonAppConfigProvider>();
		builder.Services.AddSingleton<IDirectoryDeployer, FileSystemDirectoryDeployer>();
		builder.Services.AddSingleton<IDeploymentValidator, DeploymentValidator>();
		builder.Services.AddSingleton<ISiteController, IisSiteController>();
		builder.Services.AddSingleton<DeploymentService>();

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(o =>
		{
			o.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "App API",
				Version = "v1"
			});

			// API Key auth via header X-Api-Key
			o.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
			{
				Description = "Informe a chave no header X-Api-Key",
				Name = "X-Api-Key",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey
			});

			o.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				[
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "ApiKey"
						}
					}
				] = Array.Empty<string>()
			});
		});

		var app = builder.Build();

		var apiOptions = builder.Configuration.GetSection("Api").Get<ApiOptions>() ?? new ApiOptions();
		if (string.IsNullOrWhiteSpace(apiOptions.ApiKey))
		{
			throw new InvalidOperationException("Api:ApiKey não configurada. Defina uma chave forte antes de iniciar a API.");
		}

		app.UseSwagger();
		app.UseSwaggerUI();

		app.MapGet("v1/Sites/Lista", (HttpRequest req, DeploymentService deployments) =>
		{
			// var auth = ApiAuth.Authorize(req, apiOptions);
			// if (auth is not null) return auth;

			var list = deployments.ListDeployments().Select(d => new DeploymentDto
			{
				Id = d.Id,
				NomeSite = d.NomeSite,
				Destino = d.Destino,
				Origens = new List<string>()
			}).ToList();

			return Results.Ok(list);
		});

		app.MapPost("v1/Sites/Executar", (HttpRequest req, [FromHeader(Name = "id")] string? id, DeploymentService deployments) =>
		{
			// var auth = ApiAuth.Authorize(req, apiOptions);
			// if (auth is not null) return auth;

			if (string.IsNullOrWhiteSpace(id))
			{
				return Results.BadRequest(new { message = "Header 'id' é obrigatório." });
			}

			var selected = deployments.FindById(id);
			if (selected is null)
			{
				return Results.NotFound(new { message = $"Deployment id '{id}' não encontrado." });
			}

			deployments.Run(selected);
			return Results.Ok(new { ok = true });
		});

		if (!string.IsNullOrWhiteSpace(listenUrl))
		{
			app.Urls.Clear();
			app.Urls.Add(listenUrl);
		}

		await app.RunAsync();
	}
}
