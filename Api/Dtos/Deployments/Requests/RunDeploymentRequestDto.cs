using System.Text.Json.Serialization;

namespace App.Api.Dtos.Deployments.Requests;

/// <summary>
/// DTO de request para execução de deployment.
/// </summary>
public sealed class RunDeploymentRequestDto
{
    #region Properties

    /// <summary>
    /// Identificador do deployment.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    #endregion
}
