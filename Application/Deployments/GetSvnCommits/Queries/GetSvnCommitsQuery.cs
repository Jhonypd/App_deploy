using App.Application.Messaging;

namespace App.Application.Deployments.GetSvnCommits;

#region Query

/// <summary>
/// Query para obter commits SVN de um deployment.
/// </summary>
public sealed record GetSvnCommitsQuery(string Id, int Limit) : IQuery<GetSvnCommitsResponse>;

#endregion
