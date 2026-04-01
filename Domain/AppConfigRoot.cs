namespace App.Domain;

/// <summary>
/// Modelo raiz da configuração carregada para a aplicação.
/// </summary>
public sealed class AppConfigRoot
{
	#region Properties

	/// <summary>
	/// Itens de deployment configurados.
	/// </summary>
	public List<DeploymentItem> Deployments { get; init; } = new();

	#endregion
}
