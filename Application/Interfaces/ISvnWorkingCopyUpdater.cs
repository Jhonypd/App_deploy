namespace App.Application.Interfaces;

/// <summary>
/// Contrato para atualização de revisão em working copy SVN.
/// </summary>
public interface ISvnWorkingCopyUpdater
{
    #region Methods

    /// <summary>
    /// Atualiza a working copy para uma revisão específica.
    /// </summary>
    void UpdateToRevision(string workingCopyPath, long revision);

    /// <summary>
    /// Atualiza a working copy para a revisão mais recente.
    /// </summary>
    void UpdateToHead(string workingCopyPath);

    #endregion
}
