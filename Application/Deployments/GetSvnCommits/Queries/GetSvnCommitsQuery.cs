using App.Application.Messaging;
using App.Application.Deployments.GetSvnCommits.Responses;

namespace App.Application.Deployments.GetSvnCommits.Queries;

#region Query

/// <summary>
/// Query para obter commits SVN de um deployment.
/// </summary>
public sealed record GetSvnCommitsQuery(string Id, int Limit) : IQuery<GetSvnCommitsResponse>;

#endregion
