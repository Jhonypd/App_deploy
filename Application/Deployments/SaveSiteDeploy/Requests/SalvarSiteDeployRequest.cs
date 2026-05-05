using System;
using System.Collections.Generic;

namespace App.Application.Deployments.SaveSiteDeploy.Requests;

public class SalvarSiteDeployRequest
{
    public string ProjetoIis { get; set; } = string.Empty;
    public string? Svn { get; set; }
    public string Destino { get; set; } = string.Empty;
    public string? UrlManual { get; set; }
    public bool Atualizada { get; set; }
    public IReadOnlyList<SalvarSiteOrigemRequest> Origens { get; set; } = Array.Empty<SalvarSiteOrigemRequest>();
}
