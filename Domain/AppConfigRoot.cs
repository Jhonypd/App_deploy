namespace App.Domain;

public sealed class AppConfigRoot
{
	public List<DeploymentItem> Deployments { get; init; } = new();
}
