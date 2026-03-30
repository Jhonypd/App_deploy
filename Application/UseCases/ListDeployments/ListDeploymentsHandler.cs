using App.Application.Abstractions;

namespace App.Application.UseCases.ListDeployments;

public sealed class ListDeploymentsHandler : IQueryHandler<ListDeploymentsQuery, ListDeploymentsResponse>
{
	private readonly DeploymentService _service;

	public ListDeploymentsHandler(DeploymentService service)
	{
		_service = service;
	}

	public ListDeploymentsResponse Handle(ListDeploymentsQuery query)
	{
		var items = _service.ListDeployments()
			.Select(d => new DeploymentSummary(
				d.Id,
				d.NomeSite,
				d.Svn,
				d.Destino,
				d.Origens.Select(o => new OriginSummary(o.Path, o.Conteudo)).ToList()))
			.ToList();

		return new ListDeploymentsResponse(items);
	}
}
