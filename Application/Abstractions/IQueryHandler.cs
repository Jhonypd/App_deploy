namespace App.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResult>
	where TQuery : IQuery<TResult>
{
	TResult Handle(TQuery query);
}
