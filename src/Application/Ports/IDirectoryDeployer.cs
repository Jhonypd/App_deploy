namespace App_deploy.Application.Ports;

public interface IDirectoryDeployer
{
	void DeployFromOrigins(IReadOnlyList<string> originPaths, string destinationRoot);
	string NormalizeExistingDestination(string destinationRoot);
}
