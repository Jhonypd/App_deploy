using App.Api;
using App.Api.Configurations;
using App.Application;
using App.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Ponto de entrada e composição raiz da API.
/// </summary>
public static class Program
{
    #region Constants

    private const string CorsPolicyName = "Liberado";

    #endregion

    #region Main

    /// <summary>
    /// Inicializa a API, registra dependências e configura o pipeline HTTP.
    /// </summary>
    /// <param name="args">Argumentos de inicialização.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var apiOptions = builder.Configuration.GetSection("Api").Get<ApiOptions>() ?? new ApiOptions();

        ConfigureServices(builder, apiOptions);

        var app = builder.Build();
        ValidateApiOptions(apiOptions);

        ConfigurePipeline(app);
        app.Run(apiOptions.ListenUrl);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Registra serviços da aplicação e integrações externas.
    /// </summary>
    private static void ConfigureServices(WebApplicationBuilder builder, ApiOptions apiOptions)
    {
        builder.Services.AddSingleton(apiOptions);
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddTransient<ApiKeyMiddleware>();

        builder.Services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(kvp => kvp.Value?.Errors.Count > 0)
                        .SelectMany(kvp => kvp.Value!.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Campo inválido." : e.ErrorMessage))
                        .ToList();

                    var detail = errors.Count == 0 ? null : string.Join(" | ", errors);
                    return ApiResponseFactory.Error(context.HttpContext, StatusCodes.Status400BadRequest, "Requisição inválida.", detail: detail);
                };
            });

        ConfigureCors(builder.Services);
        builder.Services.AddSwaggerConfig();
    }

    /// <summary>
    /// Registra a política de CORS permitida para os front-ends conhecidos.
    /// </summary>
    private static void ConfigureCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                policy.WithOrigins(
                        "http://192.168.0.224:7077",
                        "http://192.168.0.196:7077",
                        "http://localhost:7077",
                        "http://127.0.0.1:5055",
                        "http://localhost:5055")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    /// <summary>
    /// Configura middlewares e endpoints da aplicação.
    /// </summary>
    private static void ConfigurePipeline(WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseStatusCodePages(async statusCodeContext =>
        {
            var http = statusCodeContext.HttpContext;

            if (http.Request.Path.StartsWithSegments("/swagger"))
            {
                return;
            }

            if (http.Response.HasStarted || http.Response.ContentLength is > 0)
            {
                return;
            }

            var statusCode = http.Response.StatusCode;
            var message = statusCode switch
            {
                StatusCodes.Status404NotFound => "Não encontrado.",
                StatusCodes.Status405MethodNotAllowed => "Método não permitido.",
                StatusCodes.Status415UnsupportedMediaType => "Tipo de mídia não suportado.",
                _ => "Falha na requisição."
            };

            var result = ApiResponseFactory.Error(http, statusCode, message);
            await result.ExecuteResultAsync(new ActionContext { HttpContext = http });
        });

        app.UseCors(CorsPolicyName);
        app.UseMiddleware<ApiKeyMiddleware>();

        app.UseSwaggerConfig();
        app.MapControllers();
    }

    /// <summary>
    /// Garante que as configurações mínimas da API foram definidas.
    /// </summary>
    private static void ValidateApiOptions(ApiOptions apiOptions)
    {
        if (string.IsNullOrWhiteSpace(apiOptions.ApiKey))
        {
            throw new InvalidOperationException("Api:ApiKey não configurada. Defina uma chave forte antes de iniciar a API.");
        }
    }

    #endregion
}