using Microsoft.AspNetCore.Http;

namespace App.Api;

/// <summary>
/// Centraliza a validação de API key para as requisições da API.
/// </summary>
public static class ApiAuth
{
	#region Constants

	/// <summary>
	/// Nome do cabeçalho HTTP usado para transportar a API key.
	/// </summary>
	public const string ApiKeyHeader = "X-Api-Key";

	#endregion

	#region Public Methods

	/// <summary>
	/// Valida a API key recebida na requisição.
	/// </summary>
	public static ApiAuthFailure? Authorize(HttpRequest request, ApiOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.ApiKey))
		{
			return new ApiAuthFailure(StatusCodes.Status500InternalServerError, "API key não configurada.", "Api:ApiKey não configurada.");
		}

		if (!request.Headers.TryGetValue(ApiKeyHeader, out var provided) || provided.Count == 0)
		{
			return new ApiAuthFailure(StatusCodes.Status401Unauthorized, "Não autorizado.", $"Header '{ApiKeyHeader}' é obrigatório.");
		}

		if (!string.Equals(provided[0], options.ApiKey, StringComparison.Ordinal))
		{
			return new ApiAuthFailure(StatusCodes.Status401Unauthorized, "Não autorizado.", "API key inválida.");
		}

		return null;
	}

	#endregion
}

/// <summary>
/// Representa o erro de autenticação por API key.
/// </summary>
public sealed record ApiAuthFailure(int StatusCode, string Mensagem, string Detail);
