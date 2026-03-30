using App.Application.Abstractions;

namespace App.Application.UseCases.GetSvnCommits;

public sealed class GetSvnCommitsHandler : IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse>
{
	private readonly DeploymentService _service;

	public GetSvnCommitsHandler(DeploymentService service)
	{
		_service = service;
	}

	public GetSvnCommitsResponse Handle(GetSvnCommitsQuery query)
	{
		var selected = _service.FindById(query.Id);
		if (selected is null)
		{
			return new GetSvnCommitsResponse(Found: false, Items: Array.Empty<SvnCommitSummary>());
		}

		var currentRevision = _service.GetSvnCurrentRevision(selected);

		var items = _service.GetSvnCommits(selected, query.Limit)
			.Select(c => new SvnCommitSummary(c.Revision, c.Author, c.Date, c.Message, IsCurrent: c.Revision == currentRevision))
			.ToList();

		return new GetSvnCommitsResponse(Found: true, Items: items);
	}
}
