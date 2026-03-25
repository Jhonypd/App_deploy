using App.Application.Ports;
using Microsoft.Web.Administration;

namespace App.Infrastructure;

public sealed class IisSiteController : ISiteController
{
	public bool StopSiteIfRunning(string siteName)
	{
		if (string.IsNullOrWhiteSpace(siteName))
		{
			return false;
		}

		Console.WriteLine($"Parando site no IIS: {siteName}");
		using var manager = new ServerManager();
		var site = manager.Sites.FirstOrDefault(s => string.Equals(s.Name, siteName, StringComparison.OrdinalIgnoreCase));
		if (site is null)
		{
			throw new InvalidOperationException($"Site '{siteName}' não encontrado no IIS.");
		}

		var wasRunning = site.State == ObjectState.Started;
		if (wasRunning)
		{
			site.Stop();
			manager.CommitChanges();
			WaitForSiteState(siteName, ObjectState.Stopped, timeout: TimeSpan.FromSeconds(30));
		}
		else
		{
			Console.WriteLine("Site já estava parado.");
		}
		Console.WriteLine($"Site {site} foi parado.");
		return wasRunning;
	}

	public void StartSite(string siteName)
	{
		if (string.IsNullOrWhiteSpace(siteName))
		{
			return;
		}

		Console.WriteLine($"Iniciando site no IIS: {siteName}");
		using var manager = new ServerManager();
		var site = manager.Sites.FirstOrDefault(s => string.Equals(s.Name, siteName, StringComparison.OrdinalIgnoreCase));
		if (site is null)
		{
			throw new InvalidOperationException($"Site '{siteName}' não encontrado no IIS.");
		}

		if (site.State != ObjectState.Started)
		{
			site.Start();
			manager.CommitChanges();
			WaitForSiteState(siteName, ObjectState.Started, timeout: TimeSpan.FromSeconds(30));

			Console.WriteLine($"Site {site} iniciado novamente.");
		}
	}

	private static void WaitForSiteState(string siteName, ObjectState expected, TimeSpan timeout)
	{
		var start = DateTimeOffset.UtcNow;
		while (DateTimeOffset.UtcNow - start < timeout)
		{
			using var manager = new ServerManager();
			var site = manager.Sites.FirstOrDefault(s => string.Equals(s.Name, siteName, StringComparison.OrdinalIgnoreCase));
			if (site is not null && site.State == expected)
			{
				return;
			}
			Thread.Sleep(250);
		}

		throw new TimeoutException($"Timeout aguardando o site '{siteName}' ficar em '{expected}'.");
	}
}
