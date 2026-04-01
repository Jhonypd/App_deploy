using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace App.Api.Configurations;

/// <summary>
/// Extensões de configuração do Swagger para a API.
/// </summary>
public static class SwaggerConfig
{
    #region Public Methods

    /// <summary>
    /// Registra os serviços do Swagger com suporte a API key.
    /// </summary>
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "App-deploy API",
                Version = "v1"
            });

            // API Key via header X-Api-Key
            o.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "Informe a chave no header X-Api-Key",
                Name = "X-Api-Key",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Ativa o middleware do Swagger e da UI no pipeline HTTP.
    /// </summary>
    public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint("/swagger/v1/swagger.json", "App-deploy API v1");
        });

        return app;
    }

    #endregion
}