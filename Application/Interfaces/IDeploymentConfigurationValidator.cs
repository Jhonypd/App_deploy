using App.Domain;

namespace App.Application.Interfaces;

/// <summary>
/// Contrato para validação de itens de deployment.
/// </summary>
public interface IDeploymentConfigurationValidator
{
	#region Methods

	/// <summary>
	/// Valida as informações obrigatórias de um deployment.
	/// </summary>
	void Validate(DeploymentItem selected);

	#endregion
}
