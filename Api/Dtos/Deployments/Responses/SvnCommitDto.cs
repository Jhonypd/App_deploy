namespace App.Api.Dtos.Deployments.Responses;

/// <summary>
/// DTO de commit SVN retornado para o cliente da API.
/// </summary>
public sealed class SvnCommitDto
{
	#region Properties

	/// <summary>
	/// Número da revisão SVN.
	/// </summary>
	public long Revision { get; set; }

	/// <summary>
	/// Autor do commit.
	/// </summary>
	public string Author { get; set; } = string.Empty;

	/// <summary>
	/// Data/hora do commit.
	/// </summary>
	public DateTimeOffset Date { get; set; }

	/// <summary>
	/// Mensagem registrada no commit.
	/// </summary>
	public string Message { get; set; } = string.Empty;

	/// <summary>
	/// Indica se é a revisão atualmente aplicada.
	/// </summary>
	public bool IsCurrent { get; set; }

	#endregion
}
