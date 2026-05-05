using System.Collections.Generic;

namespace App.Application.Deployments.SaveSiteDeploy.Responses;

public class SiteDeployResponse
{
    public Guid Id { get; set; }
    public string ProjetoIis { get; set; } = string.Empty;
    public string? Svn { get; set; }
    public string Destino { get; set; } = string.Empty;
    public string? UrlManual { get; set; }
    public bool Atualizada { get; set; }
    public IReadOnlyList<SiteOrigemResponse> Origens { get; set; } = Array.Empty<SiteOrigemResponse>();
}
