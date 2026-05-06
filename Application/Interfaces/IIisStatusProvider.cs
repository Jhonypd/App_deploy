using App.Application.Deployments.GetIisStatus.Responses;
using System.Net.NetworkInformation;


namespace App.Application.Interfaces;

/// <summary>
/// Contrato para consulta do estado de sites e pools no IIS.
/// </summary>
public interface IIisStatusProvider
{
    #region Methods

    /// <summary>
    /// Obtém o status atual dos sites IIS disponíveis.
    /// </summary>
    IisStatus GetStatus();

    #endregion
}
