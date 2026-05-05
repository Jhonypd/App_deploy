using App.Application.Interfaces;
using App.Domain;
using App.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace App.Infrastructure.Configuration;

/// <summary>
/// Carrega as configurações da aplicação a partir do banco de dados e complementa se necessário.
/// </summary>
public sealed class DbAppConfigProvider : IAppConfigProvider
{
	#region Fields
	private readonly IConfiguration _configuration;
	private readonly ISiteDeployRepository _repository;
	#endregion

	#region Constructors
	/// <summary>
	/// Inicializa o provider de configuração.
	/// </summary>
	public DbAppConfigProvider(IConfiguration configuration, ISiteDeployRepository repository)
	{
		_configuration = configuration;
		_repository = repository;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Carrega e mapeia a configuração raiz da aplicação.
	/// </summary>
	public async Task<AppConfigRoot> LoadAsync()
	{
		var sites = await _repository.ObterTodos();

		return new AppConfigRoot
		{
			Deployments = sites.Select(d => new DeploymentItem
			{
				Id = d.Id.ToString(),
				NomeSite = d.ProjetoIis,
				Svn = d.Svn ?? string.Empty,
				Destino = d.Destino,
				Atualizada = d.Atualizada,
				UrlManual = d.UrlManual,
				Origins = d.Origens.Select(o => new DeploymentOrigin
				{
					Path = o.Path,
					Conteudo = o.Conteudo
				}).ToList()
			}).ToList()
		};
	}
	#endregion
}
