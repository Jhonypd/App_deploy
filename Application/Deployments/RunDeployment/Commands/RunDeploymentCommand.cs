using App.Application.Messaging;
using App.Application.Deployments.RunDeployment.Responses;

namespace App.Application.Deployments.RunDeployment.Commands;

#region Command

/// <summary>
/// Comando para iniciar o deployment de um item configurado.
/// </summary>
public sealed record RunDeploymentCommand(string Id) : ICommand<RunDeploymentResponse>;

#endregion
