using App.Application.Messaging;
using App.Application.Interfaces;

namespace App.Application.Deployments.GetIisStatus;

/// <summary>
/// Manipula a consulta de status dos sites IIS.
/// </summary>
public sealed class GetIisStatusHandler : IQueryHandler<GetIisStatusQuery, GetIisStatusResponse>
{
	#region Fields

	private readonly IIisStatusProvider _provider;

	#endregion

	#region Constructors

	/// <summary>
	/// Inicializa o handler de status IIS.
	/// </summary>
	public GetIisStatusHandler(IIisStatusProvider provider)
	{
		_provider = provider;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Executa a consulta de status IIS.
	/// </summary>
	public GetIisStatusResponse Handle(GetIisStatusQuery query)
	{
		var items = _provider.GetStatus().ToList();
		return new GetIisStatusResponse(items);
	}

	#endregion
}
