namespace App.Application.Interfaces;

/// <summary>
/// Contrato para leitura de informações da working copy SVN.
/// </summary>
public interface ISvnWorkingCopyInfoProvider
{
    #region Methods

    /// <summary>
    /// Obtém a revisão atual aplicada à working copy.
    /// </summary>
    long GetCurrentRevision(string workingCopyPath);

    #endregion
}
