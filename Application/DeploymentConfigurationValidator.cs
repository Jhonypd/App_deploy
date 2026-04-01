using App.Application.Interfaces;
using App.Domain;

namespace App.Application;

/// <summary>
/// Valida se um item de deployment possui os dados mínimos para execução.
/// </summary>
public sealed class DeploymentConfigurationValidator : IDeploymentConfigurationValidator
{
	#region Fields
	private readonly IDirectoryDeployer _directoryDeployer;
	#endregion

	#region Constructors
	/// <summary>
	/// Inicializa o validador de configurações de deployment.
	/// </summary>
	public DeploymentConfigurationValidator(IDirectoryDeployer directoryDeployer)
	{
		_directoryDeployer = directoryDeployer;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Valida identificador, destino e origens do deployment.
	/// </summary>
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

		if (selected.Origins.Count == 0)
		{
			throw new InvalidOperationException("O campo 'origens' deve conter ao menos 1 path.");
		}
	}
	#endregion
}
