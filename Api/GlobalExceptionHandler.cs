using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Api;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
	private readonly IProblemDetailsService _problemDetailsService;
	private readonly IHostEnvironment _environment;
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(
		IProblemDetailsService problemDetailsService,
		IHostEnvironment environment,
		ILogger<GlobalExceptionHandler> logger)
	{
		_problemDetailsService = problemDetailsService;
		_environment = environment;
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		var (statusCode, title) = Map(exception);

		_logger.LogError(exception, "Unhandled exception. {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

		var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

		var problemDetails = new ProblemDetails
		{
			Status = statusCode,
			Title = title,
			Detail = exception.Message,
			Instance = httpContext.Request.Path
		};

		problemDetails.Extensions["traceId"] = traceId;

		if (_environment.IsDevelopment())
		{
			problemDetails.Extensions["exception"] = exception.ToString();
		}

		httpContext.Response.StatusCode = statusCode;

		return await _problemDetailsService.TryWriteAsync(
			new ProblemDetailsContext
			{
				HttpContext = httpContext,
				ProblemDetails = problemDetails,
				Exception = exception
			});
	}

	private static (int statusCode, string title) Map(Exception exception)
	{
		return exception switch
		{
			ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
			InvalidOperationException => (StatusCodes.Status400BadRequest, "Operação inválida"),
			FileNotFoundException => (StatusCodes.Status400BadRequest, "Arquivo não encontrado"),
			DirectoryNotFoundException => (StatusCodes.Status400BadRequest, "Diretório não encontrado"),
			UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Acesso negado"),
			TimeoutException => (StatusCodes.Status504GatewayTimeout, "Tempo limite excedido"),
			_ => (StatusCodes.Status500InternalServerError, "Erro interno")
		};
	}
}
