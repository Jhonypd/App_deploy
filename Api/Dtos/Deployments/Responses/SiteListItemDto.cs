using System.Text.Json.Serialization;

namespace App.Api.Dtos.Deployments.Responses;

/// <summary>
/// DTO de listagem de site no IIS com os dados do cadastro.
/// </summary>
public sealed class SiteListItemDto
{
    #region Properties

    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("projetoIis")]
    public string ProjetoIis { get; init; } = string.Empty;

    [JsonPropertyName("porta")]
    public int Porta { get; init; } = 80;

    [JsonPropertyName("svn")]
    public string? Svn { get; init; }

    [JsonPropertyName("destino")]
    public string Destino { get; init; } = string.Empty;

    [JsonPropertyName("urlManual")]
    public string? UrlManual { get; init; }

    [JsonPropertyName("atualizada")]
    public bool Atualizada { get; init; }

    [JsonPropertyName("ImportadoIis")]
    public bool ImportadoIis { get; init; }

    [JsonPropertyName("origens")]
    public List<SiteListOriginDto> Origens { get; init; } = new();

    #endregion
}