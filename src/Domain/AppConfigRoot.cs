namespace App_deploy.Domain;

public sealed class AppConfigRoot
{
	public List<DeploymentItem> Deployments { get; init; } = new();
}
