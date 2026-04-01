using App.Application.Interfaces;
using App.Domain;
using Microsoft.Extensions.Configuration;

namespace App.Infrastructure.Configuration;

/// <summary>
/// Carrega as configurações da aplicação a partir de arquivo JSON.
/// </summary>
public sealed class JsonAppConfigProvider : IAppConfigProvider
{
	#region Fields
	private readonly IConfiguration _configuration;
	#endregion

	#region Constructors
	/// <summary>
	/// Inicializa o provider de configuração.
	/// </summary>
	public JsonAppConfigProvider(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Carrega e mapeia a configuração raiz da aplicação.
	/// </summary>
	public AppConfigRoot Load()
	{
		var settings = _configuration.Get<AppSettingsRoot>() ?? new AppSettingsRoot();
		return Map(settings);
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// Converte o modelo de configuração para o modelo de domínio.
	/// </summary>
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
					Origins = d.Origins
						.Select(o => new DeploymentOrigin
						{
							Path = o.Path ?? string.Empty,
							Conteudo = o.Conteudo
						})
						.ToList()
				})
				.ToList()
		};
	}
	#endregion

	#region Configuration Models
	private sealed class AppSettingsRoot
	{
		public List<DeploymentSettings> Deployments { get; init; } = new();
	}

	private sealed class DeploymentSettings
	{
		public string? Id { get; init; }
		public string? ProjetoIis { get; init; }
		public string? Svn { get; init; }
		[ConfigurationKeyName("origens")]
		public List<OriginSettings> Origins { get; init; } = new();
		public string? Destino { get; init; }
	}

	private sealed class OriginSettings
	{
		public string? Path { get; init; }
		public bool Conteudo { get; init; }
	}
	#endregion
}
