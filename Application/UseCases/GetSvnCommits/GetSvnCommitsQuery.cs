using App.Application.Abstractions;

namespace App.Application.UseCases.GetSvnCommits;

public sealed record GetSvnCommitsQuery(string Id, int Limit) : IQuery<GetSvnCommitsResponse>;
