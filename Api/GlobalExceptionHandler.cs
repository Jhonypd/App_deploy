using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace App.Api;

/// <summary>
/// Handler global para mapear exceções em respostas HTTP padronizadas.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
	#region Fields

	private readonly IHostEnvironment _environment;
	private readonly ILogger<GlobalExceptionHandler> _logger;

	#endregion

	#region Constructors

	/// <summary>
	/// Inicializa o handler de exceções globais.
	/// </summary>
	public GlobalExceptionHandler(
		IHostEnvironment environment,
		ILogger<GlobalExceptionHandler> logger)
	{
		_environment = environment;
		_logger = logger;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Tenta tratar uma exceção não capturada e escrever a resposta HTTP.
	/// </summary>
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		var (statusCode, title) = Map(exception);

		_logger.LogError(exception, "Unhandled exception. {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

		var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
		var detail = _environment.IsDevelopment() ? exception.ToString() : exception.Message;

		var envelope = new ApiResponse
		{
			Sucesso = false,
			Mensagem = title,
			CodHttp = statusCode,
			Detail = detail,
			Resultado = null,
			TraceId = traceId
		};

		httpContext.Response.StatusCode = statusCode;
		httpContext.Response.ContentType = "application/json; charset=utf-8";

		await httpContext.Response.WriteAsync(JsonSerializer.Serialize(envelope));
		return true;
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Mapeia uma exceção para código HTTP e título de erro amigável.
	/// </summary>
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

	#endregion
}
