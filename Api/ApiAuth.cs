using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace App.Api;

/// <summary>
/// Centraliza a validação de API key para as requisições da API.
/// </summary>
public static class ApiAuth
{
	#region Constants

	/// <summary>
	/// Nome do cabeçalho HTTP padrão usado para transportar a API key.
	/// </summary>
	public const string AuthorizationHeader = "Authorization";
	private const string BearerPrefix = "Bearer ";
	private const string Sha256Prefix = "sha256:";

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

		if (!TryGetProvidedApiKey(request, out var providedApiKey))
		{
			return new ApiAuthFailure(StatusCodes.Status401Unauthorized, "Não autorizado.", $"Header '{AuthorizationHeader}' é obrigatório (formato: 'Bearer <chave>').");
		}

		if (!IsApiKeyValid(providedApiKey, options.ApiKey))
		{
			return new ApiAuthFailure(StatusCodes.Status401Unauthorized, "Não autorizado.", "API key inválida.");
		}

		return null;
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Extrai a API key do header Authorization, preferencialmente no formato Bearer.
	/// </summary>
	private static bool TryGetProvidedApiKey(HttpRequest request, out string providedApiKey)
	{
		if (request.Headers.TryGetValue(AuthorizationHeader, out var authorizationHeader) && authorizationHeader.Count > 0)
		{
			var authorizationValue = authorizationHeader[0] ?? string.Empty;

			if (authorizationValue.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
			{
				providedApiKey = authorizationValue[BearerPrefix.Length..].Trim();
				return !string.IsNullOrWhiteSpace(providedApiKey);
			}

			providedApiKey = authorizationValue.Trim();
			return !string.IsNullOrWhiteSpace(providedApiKey);
		}

		providedApiKey = string.Empty;
		return false;
	}

	/// <summary>
	/// Valida API key recebida contra valor configurado em texto puro ou hash SHA-256.
	/// </summary>
	private static bool IsApiKeyValid(string providedApiKey, string configuredApiKey)
	{
		if (configuredApiKey.StartsWith(Sha256Prefix, StringComparison.OrdinalIgnoreCase))
		{
			var configuredHash = configuredApiKey[Sha256Prefix.Length..];
			var providedHash = ComputeSha256Hex(providedApiKey);
			return FixedTimeEquals(configuredHash, providedHash);
		}

		if (IsSha256Hex(configuredApiKey))
		{
			var providedHash = ComputeSha256Hex(providedApiKey);
			return FixedTimeEquals(configuredApiKey, providedHash);
		}

		// Compatibilidade para ambientes que ainda usam chave em texto puro.
		return string.Equals(providedApiKey, configuredApiKey, StringComparison.Ordinal);
	}

	/// <summary>
	/// Calcula o hash SHA-256 em hexadecimal minúsculo.
	/// </summary>
	private static string ComputeSha256Hex(string value)
	{
		var bytes = Encoding.UTF8.GetBytes(value);
		var hash = SHA256.HashData(bytes);
		return Convert.ToHexString(hash).ToLowerInvariant();
	}

	/// <summary>
	/// Compara dois valores em tempo constante para evitar vazamento por timing.
	/// </summary>
	private static bool FixedTimeEquals(string left, string right)
	{
		var leftBytes = Encoding.UTF8.GetBytes(left);
		var rightBytes = Encoding.UTF8.GetBytes(right);
		return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
	}

	/// <summary>
	/// Verifica se o valor está no formato SHA-256 hexadecimal.
	/// </summary>
	private static bool IsSha256Hex(string value)
	{
		if (value.Length != 64)
		{
			return false;
		}

		for (var i = 0; i < value.Length; i++)
		{
			if (!Uri.IsHexDigit(value[i]))
			{
				return false;
			}
		}

		return true;
	}

	#endregion
}

/// <summary>
/// Representa o erro de autenticação por API key.
/// </summary>
public sealed record ApiAuthFailure(int StatusCode, string Mensagem, string Detail);
