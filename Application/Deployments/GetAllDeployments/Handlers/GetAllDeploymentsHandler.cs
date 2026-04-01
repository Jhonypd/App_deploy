using App.Application.Messaging;
using App.Application.Deployments.GetAllDeployments.Queries;
using App.Application.Deployments.GetAllDeployments.Responses;

namespace App.Application.Deployments.GetAllDeployments.Handlers;

/// <summary>
/// Handler responsável por retornar todos os deployments cadastrados.
/// </summary>
public sealed class GetAllDeploymentsHandler : IQueryHandler<GetAllDeploymentsQuery, GetAllDeploymentsResponse>
{
	#region Fields
	private readonly DeploymentOrchestratorService _service;
	#endregion

	#region Constructors
	/// <summary>
	/// Inicializa o handler do caso de uso.
	/// </summary>
	public GetAllDeploymentsHandler(DeploymentOrchestratorService service)
	{
		_service = service;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Executa o caso de uso de listagem de deployments.
	/// </summary>
	public GetAllDeploymentsResponse Handle(GetAllDeploymentsQuery query)
	{
		var items = _service.GetAllDeployments()
			.Select(d => new DeploymentSummary(
				d.Id,
				d.NomeSite,
				d.Svn,
				d.Destino,
				d.Origins.Select(o => new OriginSummary(o.Path, o.Conteudo)).ToList()))
			.ToList();

		return new GetAllDeploymentsResponse(items);
	}
	#endregion
}
