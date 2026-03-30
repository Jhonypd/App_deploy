namespace App.Api;

public sealed class SvnCommitDto
{
	public long Revision { get; set; }
	public string Author { get; set; } = string.Empty;
	public DateTimeOffset Date { get; set; }
	public string Message { get; set; } = string.Empty;
	public bool IsCurrent { get; set; }
}
