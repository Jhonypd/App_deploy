using App.Application.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<IAppConfigProvider, JsonAppConfigProvider>();
		services.AddSingleton<IDirectoryDeployer, FileSystemDirectoryDeployer>();
		services.AddSingleton<ISiteController, IisSiteController>();
		services.AddSingleton<ISvnLogProvider, SvnCliLogProvider>();
		services.AddSingleton<IIisStatusProvider, IisStatusProvider>();

		return services;
	}
}
