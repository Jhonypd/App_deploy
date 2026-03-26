public sealed class SiteStatusDto
{
    public string Nome { get; set; } = "";
    public string EstadoSite { get; set; } = "";
    public string AppPool { get; set; } = "";
    public string EstadoAppPool { get; set; } = "";
    public int? Pid { get; set; }
    public double? MemoriaMb { get; set; }
    public double? Cpu { get; set; }

    public bool ProcessoAtivo { get; set; }
}