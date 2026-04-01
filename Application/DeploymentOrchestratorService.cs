using App.Application.Interfaces;
using App.Domain;

namespace App.Application;

/// <summary>
/// Orquestra o fluxo completo de deployment e operações de SVN para um item configurado.
/// </summary>
public sealed class DeploymentOrchestratorService
{
	#region Fields
	private readonly IAppConfigProvider _configProvider;
	private readonly IDeploymentConfigurationValidator _validator;
	private readonly IDirectoryDeployer _deployer;
	private readonly IIisSiteManager _siteController;
	private readonly ISvnLogProvider _svnLog;
	private readonly ISvnWorkingCopyUpdater _svnUpdater;
	private readonly ISvnWorkingCopyInfoProvider _svnInfo;
	#endregion

	#region Constructors
	/// <summary>
	/// Inicializa as dependências de orquestração de deployment.
	/// </summary>
	public DeploymentOrchestratorService(
		IAppConfigProvider configProvider,
		IDeploymentConfigurationValidator validator,
		IDirectoryDeployer deployer,
		IIisSiteManager siteController,
		ISvnLogProvider svnLog,
		ISvnWorkingCopyUpdater svnUpdater,
		ISvnWorkingCopyInfoProvider svnInfo)
	{
		_configProvider = configProvider;
		_validator = validator;
		_deployer = deployer;
		_siteController = siteController;
		_svnLog = svnLog;
		_svnUpdater = svnUpdater;
		_svnInfo = svnInfo;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Retorna todos os itens de deployment definidos na configuração.
	/// </summary>
	public IReadOnlyList<DeploymentItem> GetAllDeployments()
	{
		return _configProvider.Load().Deployments;
	}

	/// <summary>
	/// Busca um item de deployment pelo identificador.
	/// </summary>
	public DeploymentItem? FindDeploymentById(string id)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			return null;
		}

		return GetAllDeployments().FirstOrDefault(d => string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	/// Executa o deployment na revisão mais recente disponível.
	/// </summary>
	public void RunDeployment(DeploymentItem selected)
	{
		var siteWasRunning = false;
		try
		{
			_validator.Validate(selected);
			siteWasRunning = _siteController.StopSiteIfRunning(selected.NomeSite);
			if (!string.IsNullOrWhiteSpace(selected.Svn))
			{
				_svnUpdater.UpdateToHead(selected.Svn);
			}
			ExecuteDeploymentWithRetry(selected);
		}
		finally
		{
			if (siteWasRunning)
			{
				_siteController.StartSite(selected.NomeSite);
			}
		}
	}

	/// <summary>
	/// Executa o deployment em uma revisão específica do SVN.
	/// </summary>
	public void RunDeploymentAtRevision(DeploymentItem selected, long revision)
	{
		if (selected is null)
		{
			throw new ArgumentNullException(nameof(selected));
		}

		if (revision <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(revision), "A revision deve ser maior que zero.");
		}

		var siteWasRunning = false;
		try
		{
			_validator.Validate(selected);
			if (string.IsNullOrWhiteSpace(selected.Svn))
			{
				throw new InvalidOperationException("O item não possui um caminho SVN configurado.");
			}

			siteWasRunning = _siteController.StopSiteIfRunning(selected.NomeSite);
			_svnUpdater.UpdateToRevision(selected.Svn, revision);
			ExecuteDeploymentWithRetry(selected);
		}
		finally
		{
			if (siteWasRunning)
			{
				_siteController.StartSite(selected.NomeSite);
			}
		}
	}

	/// <summary>
	/// Retorna commits SVN para o item informado com limite de resultados.
	/// </summary>
	public IReadOnlyList<SvnCommit> GetSvnCommits(DeploymentItem selected, int limit)
	{
		if (selected is null)
		{
			throw new ArgumentNullException(nameof(selected));
		}

		if (string.IsNullOrWhiteSpace(selected.Svn))
		{
			return Array.Empty<SvnCommit>();
		}

		return _svnLog.GetCommits(selected.Svn, limit);
	}

	/// <summary>
	/// Retorna commits SVN desde uma revisão de referência.
	/// </summary>
	public IReadOnlyList<SvnCommit> GetSvnCommitsSinceRevision(DeploymentItem selected, long startRevision, int limit)
	{
		if (selected is null)
		{
			throw new ArgumentNullException(nameof(selected));
		}

		if (string.IsNullOrWhiteSpace(selected.Svn))
		{
			return Array.Empty<SvnCommit>();
		}

		if (startRevision <= 0)
		{
			return GetSvnCommits(selected, limit)
				.OrderByDescending(c => c.Date)
				.ThenByDescending(c => c.Revision)
				.Take(limit)
				.ToList();
		}

		var commits = _svnLog.GetCommits(selected.Svn, startRevision, limit).ToList();

		// fallback defensivo: se por algum motivo o range não retornar a revisão atual, inclui ela explicitamente
		if (!commits.Any(c => c.Revision == startRevision))
		{
			var currentCommit = _svnLog.GetCommits(selected.Svn, startRevision, limit: 1).FirstOrDefault();
			if (currentCommit is not null)
			{
				commits.Add(currentCommit);
			}
		}

		return commits
			.GroupBy(c => c.Revision)
			.Select(g => g.First())
			.OrderByDescending(c => c.Date)
			.ThenByDescending(c => c.Revision)
			.Take(limit)
			.ToList();
	}

	/// <summary>
	/// Atualiza o working copy para a revisão informada.
	/// </summary>
	public void UpdateSvnToRevision(DeploymentItem selected, long revision)
	{
		if (selected is null)
		{
			throw new ArgumentNullException(nameof(selected));
		}

		if (string.IsNullOrWhiteSpace(selected.Svn))
		{
			throw new InvalidOperationException("O item não possui um caminho SVN configurado.");
		}

		_svnUpdater.UpdateToRevision(selected.Svn, revision);
	}

	/// <summary>
	/// Obtém a revisão atual do working copy SVN.
	/// </summary>
	public long GetCurrentSvnRevision(DeploymentItem selected)
	{
		if (selected is null)
		{
			throw new ArgumentNullException(nameof(selected));
		}

		if (string.IsNullOrWhiteSpace(selected.Svn))
		{
			return 0;
		}

		return _svnInfo.GetCurrentRevision(selected.Svn);
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// Executa a cópia de arquivos com retentativa para falhas transitórias.
	/// </summary>
	private void ExecuteDeploymentWithRetry(DeploymentItem selected)
	{
		const int maxAttempts = 3;
		var delay = TimeSpan.FromSeconds(3);

		for (var attempt = 1; attempt <= maxAttempts; attempt++)
		{
			try
			{
				_deployer.DeployFromOrigins(selected.Origins, selected.Svn, selected.Destino);
				return;
			}
			catch (Exception ex) when (attempt < maxAttempts && (ex is UnauthorizedAccessException || ex is IOException))
			{
				Console.WriteLine($"Falha no deploy (tentativa {attempt}/{maxAttempts}): {ex.Message}");
				Thread.Sleep(delay);
			}
		}
	}
	#endregion
}
