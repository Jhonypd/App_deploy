namespace App.Application.Deployments.SaveSiteDeploy.Requests;

public class SalvarSiteOrigemRequest
{
    public string Path { get; set; } = string.Empty;
    public bool Conteudo { get; set; }
}
