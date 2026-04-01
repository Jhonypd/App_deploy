namespace App.Application.Deployments.RunDeployment.Responses;

#region Response Models

/// <summary>
/// Resposta da execução de deployment.
/// </summary>
public sealed record RunDeploymentResponse(bool Found, bool Started);

#endregion
