using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api;

public sealed class ApiResponse
{
	[JsonPropertyName("sucesso")]
	public bool Sucesso { get; init; }

	[JsonPropertyName("mensagem")]
	public string Mensagem { get; init; } = string.Empty;

	[JsonPropertyName("codHttp")]
	public int CodHttp { get; init; }

	[JsonPropertyName("detail")]
	public string? Detail { get; init; }

	[JsonPropertyName("resultado")]
	public object? Resultado { get; init; }

	[JsonPropertyName("traceId")]
	public string TraceId { get; init; } = string.Empty;
}

public static class ApiResponseFactory
{
	public static ObjectResult Ok(HttpContext httpContext, object? resultado, string mensagem = "OK")
	{
		return Result(httpContext, statusCode: StatusCodes.Status200OK, sucesso: true, mensagem: mensagem, detail: null, resultado: resultado);
	}

	public static ObjectResult Created(HttpContext httpContext, object? resultado, string mensagem = "Criado")
	{
		return Result(httpContext, statusCode: StatusCodes.Status201Created, sucesso: true, mensagem: mensagem, detail: null, resultado: resultado);
	}

	public static ObjectResult Error(HttpContext httpContext, int statusCode, string mensagem, string? detail = null, object? resultado = null)
	{
		return Result(httpContext, statusCode: statusCode, sucesso: false, mensagem: mensagem, detail: detail, resultado: resultado);
	}

	public static ObjectResult Result(HttpContext httpContext, int statusCode, bool sucesso, string mensagem, string? detail, object? resultado)
	{
		var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

		return new ObjectResult(new ApiResponse
		{
			Sucesso = sucesso,
			Mensagem = mensagem,
			CodHttp = statusCode,
			Detail = detail,
			Resultado = resultado,
			TraceId = traceId
		})
		{
			StatusCode = statusCode
		};
	}
}
