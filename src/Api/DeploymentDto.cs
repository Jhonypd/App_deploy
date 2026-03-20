using System.Text.Json.Serialization;

namespace App_deploy.Api;

public sealed class DeploymentDto
{
	[JsonPropertyName("id")]
	public string Id { get; init; } = string.Empty;

	[JsonPropertyName("nome_site")]
	public string NomeSite { get; init; } = string.Empty;

	[JsonPropertyName("origens")]
	public List<string> Origens { get; init; } = new();

	[JsonPropertyName("destino")]
	public string Destino { get; init; } = string.Empty;
}
