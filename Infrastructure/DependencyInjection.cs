using App.Application.Interfaces;
using App.Infrastructure.Configuration;
using App.Infrastructure.FileSystem;
using App.Infrastructure.Iis;
using App.Infrastructure.Svn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure;

/// <summary>
/// Extensões para registro dos serviços de infraestrutura.
/// </summary>
public static class DependencyInjection
{
	#region Public Methods

	/// <summary>
	/// Registra adaptadores concretos de infraestrutura no contêiner DI.
	/// </summary>
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<IAppConfigProvider, JsonAppConfigProvider>();
		services.AddSingleton<IDirectoryDeployer, FileSystemDirectoryDeployer>();
		services.AddSingleton<IIisSiteManager, IisSiteManager>();
		services.AddSingleton<ISvnLogProvider, SharpSvnLogProvider>();
		services.AddSingleton<ISvnWorkingCopyUpdater, SharpSvnWorkingCopyUpdater>();
		services.AddSingleton<ISvnWorkingCopyInfoProvider, SharpSvnWorkingCopyInfoProvider>();
		services.AddSingleton<IIisStatusProvider, IisStatusProvider>();

		return services;
	}

	#endregion
}
