namespace App.Domain;

public sealed class SvnCommit
{
	public long Revision { get; init; }
	public string Author { get; init; } = string.Empty;
	public DateTimeOffset Date { get; init; }
	public string Message { get; init; } = string.Empty;
}
