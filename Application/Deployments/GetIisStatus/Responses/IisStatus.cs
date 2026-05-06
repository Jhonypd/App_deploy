namespace App.Application.Deployments.GetIisStatus.Responses;

public sealed class IisStatus
{
    public IReadOnlyList<SiteStatus> Sites { get; init; } = [];

    public double TotalMemoriaMb => Sites.Sum(x => x.MemoriaMb ?? 0);

    public double TotalCpu => Sites.Sum(x => x.Cpu ?? 0);

    public int TotalSites => Sites.Count;

    public int TotalSitesComProcessoAtivo => Sites.Count(x => x.ProcessoAtivo);
}