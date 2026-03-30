using Microsoft.AspNetCore.Http;

namespace App.Api;

public static class ApiAuth
{
	public const string ApiKeyHeader = "X-Api-Key";

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
}

public sealed record ApiAuthFailure(int StatusCode, string Mensagem, string Detail);
