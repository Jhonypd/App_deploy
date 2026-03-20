using App_deploy.Application.Ports;
using App_deploy.Domain;

namespace App_deploy.Application;

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
			_deployer.DeployFromOrigins(selected.Origens, selected.Destino);
		}
		finally
		{
			if (siteWasRunning)
			{
				_siteController.StartSite(selected.NomeSite);
			}
		}
	}
}
