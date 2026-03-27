using App.Application.Ports;
using App.Domain;

namespace App.Application;

public sealed class DeploymentService
{
	private readonly IAppConfigProvider _configProvider;
	private readonly IDeploymentValidator _validator;
	private readonly IDirectoryDeployer _deployer;
	private readonly ISiteController _siteController;

	public DeploymentService(
		IAppConfigProvider configProvider,
		IDeploymentValidator validator,
		IDirectoryDeployer deployer,
		ISiteController siteController)
	{
		_configProvider = configProvider;
		_validator = validator;
		_deployer = deployer;
		_siteController = siteController;
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
