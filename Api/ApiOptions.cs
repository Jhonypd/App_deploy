namespace App.Api;

/// <summary>
/// Opções de configuração da API.
/// </summary>
public sealed class ApiOptions
{
	#region Properties

	/// <summary>
	/// URL base pública da API.
	/// </summary>
	public string? BaseUrl { get; init; }

	/// <summary>
	/// URL de escuta usada pelo servidor HTTP.
	/// </summary>
	public string? ListenUrl { get; init; }

	/// <summary>
	/// Chave de autenticação para consumo dos endpoints.
	/// </summary>
	public string? ApiKey { get; init; }

	#endregion
}
