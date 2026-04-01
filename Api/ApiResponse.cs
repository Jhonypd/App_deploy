using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api;

/// <summary>
/// Envelope padrão de resposta da API.
/// </summary>
public sealed class ApiResponse
{
	#region Properties

	/// <summary>
	/// Indica se a operação foi concluída com sucesso.
	/// </summary>
	[JsonPropertyName("sucesso")]
	public bool Sucesso { get; init; }

	/// <summary>
	/// Mensagem principal retornada para o cliente.
	/// </summary>
	[JsonPropertyName("mensagem")]
	public string Mensagem { get; init; } = string.Empty;

	/// <summary>
	/// Código HTTP da resposta.
	/// </summary>
	[JsonPropertyName("codHttp")]
	public int CodHttp { get; init; }

	/// <summary>
	/// Detalhes técnicos complementares.
	/// </summary>
	[JsonPropertyName("detail")]
	public string? Detail { get; init; }

	/// <summary>
	/// Conteúdo da resposta de negócio.
	/// </summary>
	[JsonPropertyName("resultado")]
	public object? Resultado { get; init; }

	/// <summary>
	/// Identificador de rastreio da requisição.
	/// </summary>
	[JsonPropertyName("traceId")]
	public string TraceId { get; init; } = string.Empty;

	#endregion
}

/// <summary>
/// Fábrica de respostas HTTP no formato padrão da API.
/// </summary>
public static class ApiResponseFactory
{
	#region Public Methods

	/// <summary>
	/// Cria uma resposta HTTP 200 de sucesso.
	/// </summary>
	public static ObjectResult Ok(HttpContext httpContext, object? resultado, string mensagem = "OK")
	{
		return Result(httpContext, statusCode: StatusCodes.Status200OK, sucesso: true, mensagem: mensagem, detail: null, resultado: resultado);
	}

	/// <summary>
	/// Cria uma resposta HTTP 201 de sucesso.
	/// </summary>
	public static ObjectResult Created(HttpContext httpContext, object? resultado, string mensagem = "Criado")
	{
		return Result(httpContext, statusCode: StatusCodes.Status201Created, sucesso: true, mensagem: mensagem, detail: null, resultado: resultado);
	}

	/// <summary>
	/// Cria uma resposta de erro com código e mensagem informados.
	/// </summary>
	public static ObjectResult Error(HttpContext httpContext, int statusCode, string mensagem, string? detail = null, object? resultado = null)
	{
		return Result(httpContext, statusCode: statusCode, sucesso: false, mensagem: mensagem, detail: detail, resultado: resultado);
	}

	/// <summary>
	/// Cria uma resposta no envelope padrão da API.
	/// </summary>
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

	#endregion
}
