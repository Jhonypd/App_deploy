namespace App.Application.UseCases.ListDeployments;

public sealed record ListDeploymentsResponse(IReadOnlyList<DeploymentSummary> Items);

public sealed record DeploymentSummary(
	string Id,
	string NomeSite,
	string Svn,
	string Destino,
	IReadOnlyList<OriginSummary> Origens);

public sealed record OriginSummary(string Path, bool Conteudo);
