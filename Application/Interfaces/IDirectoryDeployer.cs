using App.Domain;

namespace App.Application.Interfaces;

/// <summary>
/// Contrato para copiar artefatos de origem para um destino final.
/// </summary>
public interface IDirectoryDeployer
{
    #region Methods

    /// <summary>
    /// Realiza o deployment das origens para o destino informado.
    /// </summary>
    void DeployFromOrigins(IReadOnlyList<DeploymentOrigin> origins, string? svn, string destinationRoot);

    /// <summary>
    /// Normaliza e valida um caminho de destino existente.
    /// </summary>
    string NormalizeExistingDestination(string destinationRoot);

    #endregion
}