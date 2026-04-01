using App.Application.Messaging;

namespace App.Application.Deployments.RunDeployment;

#region Command

/// <summary>
/// Comando para iniciar o deployment de um item configurado.
/// </summary>
public sealed record RunDeploymentCommand(string Id) : ICommand<RunDeploymentResponse>;

#endregion
