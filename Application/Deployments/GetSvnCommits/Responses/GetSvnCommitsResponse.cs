namespace App.Application.Deployments.GetSvnCommits.Responses;

#region Response Models

/// <summary>
/// Resposta da consulta de commits SVN.
/// </summary>
public sealed record GetSvnCommitsResponse(bool Found, IReadOnlyList<SvnCommitSummary> Items);

/// <summary>
/// Visão resumida de um commit SVN.
/// </summary>
public sealed record SvnCommitSummary(long Revision, string Author, DateTimeOffset Date, string Message, bool IsCurrent);

#endregion
