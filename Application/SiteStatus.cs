namespace App.Application;

public sealed class SiteStatus
{
	public string Nome { get; init; } = string.Empty;
	public string EstadoSite { get; init; } = string.Empty;
	public string AppPool { get; init; } = string.Empty;
	public string EstadoAppPool { get; init; } = string.Empty;
	public int? Pid { get; init; }
	public double? MemoriaMb { get; init; }
	public double? Cpu { get; init; }
	public bool ProcessoAtivo { get; init; }
}
