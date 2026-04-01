using System.Text.Json.Serialization;

namespace App.Api.Dtos.Deployments.Responses;

/// <summary>
/// DTO de deployment retornado pela API.
/// </summary>
public sealed class DeploymentDto
{
	#region Properties

	/// <summary>
	/// Identificador único do deployment.
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; init; } = string.Empty;

	/// <summary>
	/// Nome do site IIS de destino.
	/// </summary>
	[JsonPropertyName("projetoIis")]
	public string NomeSite { get; init; } = string.Empty;

	/// <summary>
	/// Caminho do working copy SVN.
	/// </summary>
	[JsonPropertyName("svn")]
	public string Svn { get; set; } = string.Empty;

	/// <summary>
	/// Origens que serão copiadas para o destino.
	/// </summary>
	[JsonPropertyName("origens")]
	public List<OriginDto> Origins { get; init; } = new();

	/// <summary>
	/// Diretório de destino da publicação.
	/// </summary>
	[JsonPropertyName("destino")]
	public string Destino { get; init; } = string.Empty;

	/// <summary>
	/// Indica se a working copy local está atualizada com a última revisão.
	/// </summary>
	[JsonPropertyName("atualizada")]
	public bool Atualizada { get; init; }

	#endregion
}
