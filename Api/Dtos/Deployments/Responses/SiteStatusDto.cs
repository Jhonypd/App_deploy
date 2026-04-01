namespace App.Api.Dtos.Deployments.Responses;

/// <summary>
/// DTO de status de um site IIS e seu application pool.
/// </summary>
public sealed class SiteStatusDto
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
	/// PID do processo ativo, quando disponível.
	/// </summary>
	public int? Pid { get; init; }

	/// <summary>
	/// Memória utilizada em MB.
	/// </summary>
	public double? MemoriaMb { get; init; }

	/// <summary>
	/// CPU acumulada em segundos.
	/// </summary>
	public double? Cpu { get; init; }

	/// <summary>
	/// Indica se existe processo worker em execução.
	/// </summary>
	public bool ProcessoAtivo { get; init; }

	#endregion
}