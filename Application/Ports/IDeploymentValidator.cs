using App.Domain;

namespace App.Application.Ports;

public interface IDeploymentValidator
{
	void Validate(DeploymentItem selected);
}
