namespace App.Application.Abstractions;

public interface ICommandHandler<in TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	TResult Handle(TCommand command);
}
