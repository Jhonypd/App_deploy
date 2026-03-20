using Microsoft.Extensions.Configuration;

namespace App_deploy.Domain;

public sealed class DeploymentItem
{
	public string Id { get; init; } = string.Empty;

	[ConfigurationKeyName("nome_site")]
	public string NomeSite { get; init; } = string.Empty;

	public List<string> Origens { get; init; } = new();

	public string Destino { get; init; } = string.Empty;
}
