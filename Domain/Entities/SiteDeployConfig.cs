using System.Collections.Generic;

namespace App.Domain.Entities;

public class SiteDeployConfig
{
    public string Id { get; set; } = string.Empty;
    public string ProjetoIis { get; set; } = string.Empty;
    public string? Svn { get; set; }
    public string Destino { get; set; } = string.Empty;
    public string? UrlManual { get; set; }
    public bool Atualizada { get; set; }

    public ICollection<SiteOrigemConfig> Origens { get; set; } = new List<SiteOrigemConfig>();
}
