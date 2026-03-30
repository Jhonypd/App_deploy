using App.Application.Abstractions;

namespace App.Application.UseCases.RunDeployment;

public sealed record RunDeploymentCommand(string Id) : ICommand<RunDeploymentResponse>;
