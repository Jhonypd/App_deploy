namespace App.Application.Deployments.SaveSiteDeploy.Responses;

public class SiteOrigemResponse
{
    public Guid Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public bool Conteudo { get; set; }
}
