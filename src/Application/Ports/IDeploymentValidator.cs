using App_deploy.Domain;

namespace App_deploy.Application.Ports;

public interface IDeploymentValidator
{
	void Validate(DeploymentItem selected);
}
