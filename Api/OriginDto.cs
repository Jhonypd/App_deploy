using System.Text.Json.Serialization;

namespace App.Api;

public sealed class OriginDto
{
	[JsonPropertyName("path")]
	public string Path { get; init; } = string.Empty;

	[JsonPropertyName("conteudo")]
	public bool Conteudo { get; init; }
}
