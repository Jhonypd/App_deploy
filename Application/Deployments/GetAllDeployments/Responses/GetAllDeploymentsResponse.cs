namespace App.Application.Deployments.GetAllDeployments.Responses;

/// <summary>
/// Resposta da listagem de deployments.
/// </summary>
#region Response Models
public sealed record GetAllDeploymentsResponse(IReadOnlyList<DeploymentSummary> Items);

/// <summary>
/// Visão resumida de um item de deployment.
/// </summary>
public sealed record DeploymentSummary(
	string Id,
	string NomeSite,
	string Svn,
	string Destino,
	IReadOnlyList<OriginSummary> Origins,
	bool Atualizada);

/// <summary>
/// Visão resumida de uma origem de deployment.
/// </summary>
public sealed record OriginSummary(string Path, bool Conteudo);
#endregion
