using App.Application.Abstractions;
using App.Application.Ports;

namespace App.Application.UseCases.GetIisStatus;

public sealed class GetIisStatusHandler : IQueryHandler<GetIisStatusQuery, GetIisStatusResponse>
{
	private readonly IIisStatusProvider _provider;

	public GetIisStatusHandler(IIisStatusProvider provider)
	{
		_provider = provider;
	}

	public GetIisStatusResponse Handle(GetIisStatusQuery query)
	{
		var items = _provider.GetStatus().ToList();
		return new GetIisStatusResponse(items);
	}
}
