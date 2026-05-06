using System.Text.Json.Serialization;

namespace App.Api.Dtos.Deployments.Responses;

/// <summary>
/// DTO de origem usado na listagem de sites.
/// </summary>
public sealed class SiteListOriginDto
{
    #region Properties

    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("path")]
    public string Path { get; init; } = string.Empty;

    [JsonPropertyName("conteudo")]
    public bool Conteudo { get; init; }

    #endregion
}