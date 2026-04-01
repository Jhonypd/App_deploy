using System.Text.Json.Serialization;

namespace App.Api.Dtos.Deployments.Responses;

/// <summary>
/// DTO que representa uma origem de arquivos para deployment.
/// </summary>
public sealed class OriginDto
{
	#region Properties

	/// <summary>
	/// Caminho da origem dos arquivos.
	/// </summary>
	[JsonPropertyName("path")]
	public string Path { get; init; } = string.Empty;

	/// <summary>
	/// Indica se deve copiar apenas conteúdo da pasta.
	/// </summary>
	[JsonPropertyName("conteudo")]
	public bool Conteudo { get; init; }

	#endregion
}
