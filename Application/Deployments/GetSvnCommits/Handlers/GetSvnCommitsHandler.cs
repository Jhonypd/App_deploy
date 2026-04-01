using App.Application.Messaging;
using App.Application.Deployments.GetSvnCommits.Queries;
using App.Application.Deployments.GetSvnCommits.Responses;

namespace App.Application.Deployments.GetSvnCommits.Handlers;

/// <summary>
/// Manipula a consulta de commits SVN de um deployment.
/// </summary>
public sealed class GetSvnCommitsHandler : IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse>
{
	#region Fields

	private readonly DeploymentOrchestratorService _service;

	#endregion

	#region Constructors

	/// <summary>
	/// Inicializa o handler de commits SVN.
	/// </summary>
	public GetSvnCommitsHandler(DeploymentOrchestratorService service)
	{
		_service = service;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Executa a consulta de commits para o deployment informado.
	/// </summary>
	public GetSvnCommitsResponse Handle(GetSvnCommitsQuery query)
	{
		var selected = _service.FindDeploymentById(query.Id);
		if (selected is null)
		{
			return new GetSvnCommitsResponse(Found: false, Items: Array.Empty<SvnCommitSummary>());
		}

		var currentRevision = _service.GetCurrentSvnRevision(selected);

		var items = _service.GetSvnCommitsSinceRevision(selected, currentRevision, query.Limit)
			.Select(c => new SvnCommitSummary(c.Revision, c.Author, c.Date, c.Message, IsCurrent: c.Revision == currentRevision))
			.ToList();

		return new GetSvnCommitsResponse(Found: true, Items: items);
	}

	#endregion
}
