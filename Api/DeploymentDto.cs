using System.Text.Json.Serialization;

namespace App.Api;

public sealed class DeploymentDto
{
	[JsonPropertyName("id")]
	public string Id { get; init; } = string.Empty;

	[JsonPropertyName("projetoIis")]
	public string NomeSite { get; init; } = string.Empty;

	[JsonPropertyName("svn")]
	public string Svn { get; set; } = string.Empty;

	[JsonPropertyName("origens")]
	public List<OriginDto> Origens { get; init; } = new();

	[JsonPropertyName("destino")]
	public string Destino { get; init; } = string.Empty;
}
