using Microsoft.AspNetCore.Http;

namespace App_deploy.Api;

public static class ApiAuth
{
	public const string ApiKeyHeader = "X-Api-Key";

	public static IResult? Authorize(HttpRequest request, ApiOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.ApiKey))
		{
			return Results.Problem("API key não configurada (Api:ApiKey).", statusCode: StatusCodes.Status500InternalServerError);
		}

		if (!request.Headers.TryGetValue(ApiKeyHeader, out var provided) || provided.Count == 0)
		{
			return Results.Unauthorized();
		}

		if (!string.Equals(provided[0], options.ApiKey, StringComparison.Ordinal))
		{
			return Results.Unauthorized();
		}

		return null;
	}
}
