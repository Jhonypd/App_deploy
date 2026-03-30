using App.Application.Ports;
using App.Domain;
using Microsoft.Extensions.Configuration;

namespace App.Infrastructure;

public sealed class JsonAppConfigProvider : IAppConfigProvider
{
	private readonly IConfiguration _configuration;

	public JsonAppConfigProvider(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public AppConfigRoot Load()
	{
		var settings = _configuration.Get<AppSettingsRoot>() ?? new AppSettingsRoot();
		return Map(settings);
	}

	private static AppConfigRoot Map(AppSettingsRoot root)
	{
		return new AppConfigRoot
		{
			Deployments = root.Deployments
				.Select(d => new DeploymentItem
				{
					Id = d.Id ?? string.Empty,
					NomeSite = d.ProjetoIis ?? string.Empty,
					Svn = d.Svn ?? string.Empty,
					Destino = d.Destino ?? string.Empty,
					Origens = d.Origens
						.Select(o => new OrigensConfig
						{
							Path = o.Path ?? string.Empty,
							Conteudo = o.Conteudo
						})
						.ToList()
				})
				.ToList()
		};
	}

	private sealed class AppSettingsRoot
	{
		public List<DeploymentSettings> Deployments { get; init; } = new();
	}

	private sealed class DeploymentSettings
	{
		public string? Id { get; init; }
		public string? ProjetoIis { get; init; }
		public string? Svn { get; init; }
		public List<OriginSettings> Origens { get; init; } = new();
		public string? Destino { get; init; }
	}

	private sealed class OriginSettings
	{
		public string? Path { get; init; }
		public bool Conteudo { get; init; }
	}
}
