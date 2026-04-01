using App.Application.Messaging;

namespace App.Application.Deployments.GetIisStatus;

#region Query

/// <summary>
/// Query para obter o status atual dos sites IIS.
/// </summary>
public sealed record GetIisStatusQuery() : IQuery<GetIisStatusResponse>;

#endregion
