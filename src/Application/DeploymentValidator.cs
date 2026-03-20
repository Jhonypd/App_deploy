using App_deploy.Application.Ports;
using App_deploy.Domain;

namespace App_deploy.Application;

public sealed class DeploymentValidator : IDeploymentValidator
{
	private readonly IDirectoryDeployer _directoryDeployer;

	public DeploymentValidator(IDirectoryDeployer directoryDeployer)
	{
		_directoryDeployer = directoryDeployer;
	}

	public void Validate(DeploymentItem selected)
	{
		if (string.IsNullOrWhiteSpace(selected.Id))
		{
			throw new InvalidOperationException("O campo 'id' é obrigatório.");
		}

		if (string.IsNullOrWhiteSpace(selected.Destino))
		{
			throw new InvalidOperationException("O campo 'destino' é obrigatório.");
		}

		_directoryDeployer.NormalizeExistingDestination(selected.Destino);

		if (selected.Origens.Count == 0)
		{
			throw new InvalidOperationException("O campo 'origens' deve conter ao menos 1 path.");
		}
	}
}
