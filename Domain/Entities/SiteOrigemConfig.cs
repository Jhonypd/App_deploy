namespace App.Domain.Entities;

public class SiteOrigemConfig
{
    public string Id { get; set; } = string.Empty;
    public string SiteDeployConfigId { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public bool Conteudo { get; set; }

    public SiteDeployConfig SiteDeployConfig { get; set; } = null!;
}
