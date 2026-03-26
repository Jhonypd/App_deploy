using Microsoft.Web.Administration;
using System.Diagnostics;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
public class IisMonitorService
{
    public List<SiteStatusDto> GetStatus()
    {
        using var manager = new ServerManager();

        var result = new List<SiteStatusDto>();

        foreach (var site in manager.Sites)
        {
            var app = site.Applications["/"];
            var poolName = app.ApplicationPoolName;

            var pool = manager.ApplicationPools.FirstOrDefault(p =>
                string.Equals(p.Name, poolName, StringComparison.OrdinalIgnoreCase));

            var status = new SiteStatusDto
            {
                Nome = site.Name,
                EstadoSite = site.State.ToString(),
                AppPool = poolName,
                EstadoAppPool = pool?.State.ToString() ?? "Unknown"
            };

            try
            {
                var worker = manager.WorkerProcesses
                    .FirstOrDefault(wp => string.Equals(wp.AppPoolName, poolName, StringComparison.OrdinalIgnoreCase));

                status.ProcessoAtivo = worker is not null;
                status.Pid = worker?.ProcessId;

                if (worker is not null)
                {
                    try
                    {
                        var processo = Process.GetProcessById(worker.ProcessId);
                        status.MemoriaMb = Math.Round(processo.WorkingSet64 / 1024.0 / 1024.0, 2);
                        status.Cpu = Math.Round(processo.TotalProcessorTime.TotalSeconds, 2);
                    }
                    catch
                    {
                        // best-effort: PID já foi identificado via IIS
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

}