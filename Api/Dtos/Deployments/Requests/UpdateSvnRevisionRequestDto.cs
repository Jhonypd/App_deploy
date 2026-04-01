using System.Text.Json.Serialization;

namespace App.Api.Dtos.Deployments.Requests;

/// <summary>
/// DTO de request para atualização da revisão SVN.
/// </summary>
public sealed class UpdateSvnRevisionRequestDto
{
    #region Properties

    /// <summary>
    /// Identificador do deployment.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Revisão SVN alvo para atualização.
    /// </summary>
    [JsonPropertyName("revision")]
    public long Revision { get; init; }

    #endregion
}
