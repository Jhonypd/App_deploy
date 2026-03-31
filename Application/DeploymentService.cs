using App.Application.Ports;
using App.Domain;

namespace App.Application;

public sealed class DeploymentService
{
	private readonly IAppConfigProvider _configProvider;
	private readonly IDeploymentValidator _validator;
	private readonly IDirectoryDeployer _deployer;
	private readonly ISiteController _siteController;
	private readonly ISvnLogProvider _svnLog;
	private readonly ISvnWorkingCopyUpdater _svnUpdater;
	private readonly ISvnWorkingCopyInfoProvider _svnInfo;

	public DeploymentService(
		IAppConfigProvider configProvider,
		IDeploymentValidator validator,
		IDirectoryDeployer deployer,
		ISiteController siteController,
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

	public IReadOnlyList<DeploymentItem> ListDeployments()
	{
		return _configProvider.Load().Deployments;
	}

	public DeploymentItem? FindById(string id)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			return null;
		}

		return ListDeployments().FirstOrDefault(d => string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase));
	}

	public void Run(DeploymentItem selected)
	{
		var siteWasRunning = false;
		try
		{
			_validator.Validate(selected);
			siteWasRunning = _siteController.StopSiteIfRunning(selected.NomeSite);
			DeployWithRetry(selected);
		}
		finally
		{
			if (siteWasRunning)
			{
				_siteController.StartSite(selected.NomeSite);
			}
		}
	}

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

	public IReadOnlyList<SvnCommit> GetSvnCommitsFromRevision(DeploymentItem selected, long startRevision, int limit)
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

	public long GetSvnCurrentRevision(DeploymentItem selected)
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

	private void DeployWithRetry(DeploymentItem selected)
	{
		const int maxAttempts = 3;
		var delay = TimeSpan.FromSeconds(3);

		for (var attempt = 1; attempt <= maxAttempts; attempt++)
		{
			try
			{
				_deployer.DeployFromOrigins(selected.Origens, selected.Svn, selected.Destino);
				return;
			}
			catch (Exception ex) when (attempt < maxAttempts && (ex is UnauthorizedAccessException || ex is IOException))
			{
				Console.WriteLine($"Falha no deploy (tentativa {attempt}/{maxAttempts}): {ex.Message}");
				Thread.Sleep(delay);
			}
		}
	}
}
