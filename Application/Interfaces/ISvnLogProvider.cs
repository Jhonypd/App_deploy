using App.Domain;

namespace App.Application.Interfaces;

/// <summary>
/// Contrato para leitura de histórico de commits SVN.
/// </summary>
public interface ISvnLogProvider
{
	#region Methods

	/// <summary>
	/// Obtém os commits mais recentes do caminho informado.
	/// </summary>
	IReadOnlyList<SvnCommit> GetCommits(string workingCopyPathOrUrl, int limit);

	/// <summary>
	/// Obtém commits a partir de uma revisão inicial.
	/// </summary>
	IReadOnlyList<SvnCommit> GetCommits(string workingCopyPathOrUrl, long startRevision, int limit);

	#endregion
}
