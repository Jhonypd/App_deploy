using App.Application.Messaging;
using App.Application.Deployments.GetIisStatus.Responses;

namespace App.Application.Deployments.GetIisStatus.Queries;

#region Query

/// <summary>
/// Query para obter o status atual dos sites IIS.
/// </summary>
public sealed record GetIisStatusQuery() : IQuery<GetIisStatusResponse>;

#endregion
