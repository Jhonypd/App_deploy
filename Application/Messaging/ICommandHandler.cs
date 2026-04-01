namespace App.Application.Messaging;

/// <summary>
/// Define o manipulador de um comando.
/// </summary>
public interface ICommandHandler<in TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	#region Methods

	/// <summary>
	/// Processa o comando informado e retorna o resultado.
	/// </summary>
	TResult Handle(TCommand command);

	#endregion
}
