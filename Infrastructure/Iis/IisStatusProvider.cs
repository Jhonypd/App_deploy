using System.Diagnostics;
using System.Runtime.Versioning;
using App.Application;
using App.Application.Interfaces;
using Microsoft.Web.Administration;

namespace App.Infrastructure.Iis;

[SupportedOSPlatform("windows")]
public sealed class IisStatusProvider : IIisStatusProvider
{
	#region Public Methods

	/// <summary>
	/// Obtém o status de sites e pools configurados no IIS local.
	/// </summary>
	public IReadOnlyList<SiteStatus> GetStatus()
	{
		using var manager = new ServerManager();
		var result = new List<SiteStatus>();

		foreach (var site in manager.Sites)
		{
			var app = site.Applications["/"];
			var poolName = app.ApplicationPoolName;

			var pool = manager.ApplicationPools.FirstOrDefault(p =>
				string.Equals(p.Name, poolName, StringComparison.OrdinalIgnoreCase));

			var status = new SiteStatus
			{
				Nome = site.Name,
				EstadoSite = site.State.ToString(),
				AppPool = poolName,
				EstadoAppPool = pool?.State.ToString() ?? "Unknown",
				ProcessoAtivo = false
			};

			try
			{
				var worker = manager.WorkerProcesses.FirstOrDefault(wp =>
					string.Equals(wp.AppPoolName, poolName, StringComparison.OrdinalIgnoreCase));

				if (worker is not null)
				{
					status = new SiteStatus
					{
						Nome = status.Nome,
						EstadoSite = status.EstadoSite,
						AppPool = status.AppPool,
						EstadoAppPool = status.EstadoAppPool,
						ProcessoAtivo = true,
						Pid = worker.ProcessId
					};

					try
					{
						var processo = Process.GetProcessById(worker.ProcessId);
						status = new SiteStatus
						{
							Nome = status.Nome,
							EstadoSite = status.EstadoSite,
							AppPool = status.AppPool,
							EstadoAppPool = status.EstadoAppPool,
							ProcessoAtivo = status.ProcessoAtivo,
							Pid = status.Pid,
							MemoriaMb = Math.Round(processo.WorkingSet64 / 1024.0 / 1024.0, 2),
							Cpu = Math.Round(processo.TotalProcessorTime.TotalSeconds, 2)
						};
					}
					catch
					{
						// best-effort
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao buscar processo do pool '{poolName}': {ex.Message}");
			}

			result.Add(status);
		}

		return result;
	}

	#endregion
}
