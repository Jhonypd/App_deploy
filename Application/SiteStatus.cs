namespace App.Application;

/// <summary>
/// Representa o estado operacional de um site IIS e seu worker process.
/// </summary>
public sealed class SiteStatus
{
	#region Properties

	/// <summary>
	/// Nome do site IIS.
	/// </summary>
	public string Nome { get; init; } = string.Empty;

	/// <summary>
	/// Estado atual do site IIS.
	/// </summary>
	public string EstadoSite { get; init; } = string.Empty;

	/// <summary>
	/// Nome do application pool associado.
	/// </summary>
	public string AppPool { get; init; } = string.Empty;

	/// <summary>
	/// Estado atual do application pool.
	/// </summary>
	public string EstadoAppPool { get; init; } = string.Empty;

	/// <summary>
	/// PID do processo worker quando disponível.
	/// </summary>
	public int? Pid { get; init; }

	/// <summary>
	/// Memória em MB do processo worker.
	/// </summary>
	public double? MemoriaMb { get; init; }

	/// <summary>
	/// CPU acumulada em segundos do processo worker.
	/// </summary>
	public double? Cpu { get; init; }

	/// <summary>
	/// Indica se há processo worker ativo.
	/// </summary>
	public bool ProcessoAtivo { get; init; }

	#endregion
}
