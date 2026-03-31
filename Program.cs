using App.Api;
using App.Api.Configurations;
using App.Application;
using App.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Config (já vem automático, nem precisa recriar)
var apiOptions = builder.Configuration.GetSection("Api").Get<ApiOptions>() ?? new ApiOptions();

builder.Services.AddSingleton(apiOptions);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Necessário para o middleware UseExceptionHandler (mesmo com handler customizado)
builder.Services.AddProblemDetails();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddTransient<ApiKeyMiddleware>();

// MVC Controllers
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(o =>
    {
        o.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(kvp => kvp.Value?.Errors.Count > 0)
                .SelectMany(kvp => kvp.Value!.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Campo inválido." : e.ErrorMessage))
                .ToList();

            var detail = errors.Count == 0 ? null : string.Join(" | ", errors);
            return ApiResponseFactory.Error(context.HttpContext, StatusCodes.Status400BadRequest, "Requisição inválida.", detail: detail);
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("Liberado", policy =>
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

// Swagger
builder.Services.AddSwaggerConfig();

var app = builder.Build();

if (string.IsNullOrWhiteSpace(apiOptions.ApiKey))
{
    throw new InvalidOperationException("Api:ApiKey não configurada. Defina uma chave forte antes de iniciar a API.");
}

app.UseExceptionHandler();

app.UseStatusCodePages(async statusCodeContext =>
{
    var http = statusCodeContext.HttpContext;

    // não interfere no Swagger UI/JSON
    if (http.Request.Path.StartsWithSegments("/swagger"))
    {
        return;
    }

    // se já tem body, não sobrescreve
    if (http.Response.HasStarted || http.Response.ContentLength is > 0)
    {
        return;
    }

    var statusCode = http.Response.StatusCode;
    var mensagem = statusCode switch
    {
        StatusCodes.Status404NotFound => "Não encontrado.",
        StatusCodes.Status405MethodNotAllowed => "Método não permitido.",
        StatusCodes.Status415UnsupportedMediaType => "Tipo de mídia não suportado.",
        _ => "Falha na requisição."
    };

    var result = ApiResponseFactory.Error(http, statusCode, mensagem);
    await result.ExecuteResultAsync(new ActionContext { HttpContext = http });
});

app.UseCors("Liberado");

app.UseMiddleware<ApiKeyMiddleware>();

// Swagger + Controllers
app.UseSwaggerConfig();
app.MapControllers();

app.Run(apiOptions.ListenUrl);