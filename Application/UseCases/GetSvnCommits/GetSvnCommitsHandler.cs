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

		var items = _service.GetSvnCommits(selected, query.Limit)
			.Select(c => new SvnCommitSummary(c.Revision, c.Author, c.Date, c.Message))
			.ToList();

		return new GetSvnCommitsResponse(Found: true, Items: items);
	}
}
