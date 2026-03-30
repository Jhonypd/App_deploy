namespace App.Application.UseCases.GetSvnCommits;

public sealed record GetSvnCommitsResponse(bool Found, IReadOnlyList<SvnCommitSummary> Items);

public sealed record SvnCommitSummary(long Revision, string Author, DateTimeOffset Date, string Message);
