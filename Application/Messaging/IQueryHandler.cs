namespace App.Application.Messaging;

/// <summary>
/// Define o manipulador de uma consulta.
/// </summary>
public interface IQueryHandler<in TQuery, TResult>
	where TQuery : IQuery<TResult>
{
	#region Methods

	/// <summary>
	/// Processa a consulta informada e retorna o resultado.
	/// </summary>
	TResult Handle(TQuery query);

	#endregion
}
