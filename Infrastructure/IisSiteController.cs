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

		var rootApp = site.Applications["/"];
		var appPoolName = rootApp?.ApplicationPoolName;
		if (string.IsNullOrWhiteSpace(appPoolName))
		{
			throw new InvalidOperationException($"Não foi possível determinar o Application Pool do site '{siteName}'.");
		}
		var appPool = manager.ApplicationPools.FirstOrDefault(p => string.Equals(p.Name, appPoolName, StringComparison.OrdinalIgnoreCase));
		if (appPool is null)
		{
			throw new InvalidOperationException($"Application Pool '{appPoolName}' não encontrado para o site '{siteName}'.");
		}

		var wasRunning = site.State == ObjectState.Started;
		var poolWasRunning = appPool.State == ObjectState.Started;
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

		if (poolWasRunning)
		{
			Console.WriteLine($"Parando Application Pool no IIS: {appPoolName}");
			appPool.Stop();
			manager.CommitChanges();
			WaitForAppPoolState(appPoolName, ObjectState.Stopped, timeout: TimeSpan.FromSeconds(30));
		}
		else
		{
			Console.WriteLine("Application Pool já estava parado.");
		}

		Console.WriteLine($"Site {site} foi parado.");
		return wasRunning || poolWasRunning;
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

		var rootApp = site.Applications["/"];
		var appPoolName = rootApp?.ApplicationPoolName;
		if (!string.IsNullOrWhiteSpace(appPoolName))
		{
			var appPool = manager.ApplicationPools.FirstOrDefault(p => string.Equals(p.Name, appPoolName, StringComparison.OrdinalIgnoreCase));
			if (appPool is not null && appPool.State != ObjectState.Started)
			{
				Console.WriteLine($"Iniciando Application Pool no IIS: {appPoolName}");
				appPool.Start();
				manager.CommitChanges();
				WaitForAppPoolState(appPoolName, ObjectState.Started, timeout: TimeSpan.FromSeconds(30));
			}
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

	private static void WaitForAppPoolState(string appPoolName, ObjectState expected, TimeSpan timeout)
	{
		var start = DateTimeOffset.UtcNow;
		while (DateTimeOffset.UtcNow - start < timeout)
		{
			using var manager = new ServerManager();
			var pool = manager.ApplicationPools.FirstOrDefault(p => string.Equals(p.Name, appPoolName, StringComparison.OrdinalIgnoreCase));
			if (pool is not null && pool.State == expected)
			{
				return;
			}
			Thread.Sleep(250);
		}

		throw new TimeoutException($"Timeout aguardando o Application Pool '{appPoolName}' ficar em '{expected}'.");
	}
}
