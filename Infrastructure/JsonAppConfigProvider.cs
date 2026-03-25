using App.Application.Ports;
using App.Domain;
using Microsoft.Extensions.Configuration;

namespace App.Infrastructure;

public sealed class JsonAppConfigProvider : IAppConfigProvider
{
	public AppConfigRoot Load()
	{
		var config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
			.Build();

		return config.Get<AppConfigRoot>() ?? new AppConfigRoot();
	
	}
}
