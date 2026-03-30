using App.Application.Abstractions;

namespace App.Application.UseCases.RunDeployment;

public sealed class RunDeploymentHandler : ICommandHandler<RunDeploymentCommand, RunDeploymentResponse>
{
	private readonly DeploymentService _service;

	public RunDeploymentHandler(DeploymentService service)
	{
		_service = service;
	}

	public RunDeploymentResponse Handle(RunDeploymentCommand command)
	{
	var selected = _service.FindById(command.Id);
		if (selected is null)
		{
			return new RunDeploymentResponse(Found: false, Started: false);
		}

		_service.Run(selected);
		return new RunDeploymentResponse(Found: true, Started: true);
	}
}
