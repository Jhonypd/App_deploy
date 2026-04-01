using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api;

/// <summary>
/// Middleware responsável por validar API key antes de processar as requisições.
/// </summary>
public sealed class ApiKeyMiddleware : IMiddleware
{
	#region Fields

	private readonly ApiOptions _options;

	#endregion

	#region Constructors

	/// <summary>
	/// Inicializa o middleware com as opções de autenticação.
	/// </summary>
	public ApiKeyMiddleware(ApiOptions options)
	{
		_options = options;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Intercepta a requisição e aplica validação de API key quando necessário.
	/// </summary>
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		if (HttpMethods.IsOptions(context.Request.Method))
		{
			await next(context);
			return;
		}

		// libera Swagger sem exigir API key
		if (context.Request.Path.StartsWithSegments("/swagger"))
		{
			await next(context);
			return;
		}

		var authResult = ApiAuth.Authorize(context.Request, _options);
		if (authResult is not null)
		{
			var result = ApiResponseFactory.Error(
				context,
				statusCode: authResult.StatusCode,
				mensagem: authResult.Mensagem,
				detail: authResult.Detail);

			context.Response.StatusCode = authResult.StatusCode;
			await result.ExecuteResultAsync(new ActionContext { HttpContext = context });
			return;
		}

		await next(context);
	}

	#endregion
}
