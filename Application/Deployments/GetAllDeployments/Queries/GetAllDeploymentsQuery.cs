using App.Application.Messaging;
using App.Application.Deployments.GetAllDeployments.Responses;

namespace App.Application.Deployments.GetAllDeployments.Queries;

/// <summary>
/// Query para obter todos os deployments configurados.
/// </summary>
#region Query
public sealed record GetAllDeploymentsQuery() : IQuery<GetAllDeploymentsResponse>;
#endregion
