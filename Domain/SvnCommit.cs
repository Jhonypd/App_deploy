namespace App.Domain;

/// <summary>
/// Representa um commit do histórico SVN.
/// </summary>
public sealed class SvnCommit
{
	#region Properties

	/// <summary>
	/// Número da revisão SVN.
	/// </summary>
	public long Revision { get; init; }

	/// <summary>
	/// Autor responsável pelo commit.
	/// </summary>
	public string Author { get; init; } = string.Empty;

	/// <summary>
	/// Data e hora do commit.
	/// </summary>
	public DateTimeOffset Date { get; init; }

	/// <summary>
	/// Mensagem de commit.
	/// </summary>
	public string Message { get; init; } = string.Empty;

	#endregion
}
