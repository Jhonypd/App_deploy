namespace App.Application.Messaging;

/// <summary>
/// Representa um comando da aplicação que retorna um resultado.
/// </summary>
public interface ICommand<out TResult>
{
    #region Marker Interface

    #endregion
}
