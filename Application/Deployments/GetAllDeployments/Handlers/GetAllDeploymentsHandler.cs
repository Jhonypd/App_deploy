using App.Application.Messaging;
using App.Application.Deployments.GetAllDeployments.Queries;
using App.Application.Deployments.GetAllDeployments.Responses;
using App.Application.Interfaces;

namespace App.Application.Deployments.GetAllDeployments.Handlers;

/// <summary>
/// Handler responsável por retornar apenas os deployments que já existem no IIS.
/// </summary>
public sealed class GetAllDeploymentsHandler : IQueryHandler<GetAllDeploymentsQuery, GetAllDeploymentsResponse>
{
	#region Fields
	private readonly DeploymentOrchestratorService _service;
	private readonly IIisStatusProvider _iisStatusProvider;
	#endregion

	#region Constructors
	/// <summary>
	/// Inicializa o handler do caso de uso.
	/// </summary>
	public GetAllDeploymentsHandler(DeploymentOrchestratorService service, IIisStatusProvider iisStatusProvider)
	{
		_service = service;
		_iisStatusProvider = iisStatusProvider;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Executa o caso de uso de listagem de deployments já cadastrados no IIS.
	/// </summary>
	public GetAllDeploymentsResponse Handle(GetAllDeploymentsQuery query)
	{
		var iisSiteNames = _iisStatusProvider.GetStatus().Sites
			.Select(s => s.Nome)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		var items = _service.GetAllDeploymentsAsync().GetAwaiter().GetResult()
			.Where(d => iisSiteNames.Contains(d.NomeSite))
			.Select(BuildDeploymentSummary)
			.ToList();

		return new GetAllDeploymentsResponse(items);
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// Monta o resumo de deployment incluindo status de atualização SVN.
	/// </summary>
	private DeploymentSummary BuildDeploymentSummary(App.Domain.DeploymentItem deployment)
	{
		var isUpdated = false;

		if (!string.IsNullOrWhiteSpace(deployment.Svn))
		{
			try
			{
				var currentRevision = _service.GetCurrentSvnRevision(deployment);
				var latestRevision = _service.GetSvnCommits(deployment, 1).FirstOrDefault()?.Revision;
				isUpdated = latestRevision.HasValue && currentRevision >= latestRevision.Value;
			}
			catch
			{
				// Falhas de acesso ao SVN não devem interromper a listagem de aplicações.
			}
		}

		return new DeploymentSummary(
			deployment.Id,
			deployment.NomeSite,
			deployment.Svn,
			deployment.Destino,
			deployment.Origins.Select(o => new OriginSummary(o.Path, o.Conteudo)).ToList(),
			isUpdated);
	}
	#endregion
}
