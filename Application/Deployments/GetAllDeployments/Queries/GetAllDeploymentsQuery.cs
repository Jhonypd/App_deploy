using App.Application.Messaging;

namespace App.Application.Deployments.GetAllDeployments;

/// <summary>
/// Query para obter todos os deployments configurados.
/// </summary>
#region Query
public sealed record GetAllDeploymentsQuery() : IQuery<GetAllDeploymentsResponse>;
#endregion
