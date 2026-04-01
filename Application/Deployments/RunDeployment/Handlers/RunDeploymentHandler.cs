using App.Application.Messaging;

namespace App.Application.Deployments.RunDeployment;

/// <summary>
/// Manipula a execução de deployment para um item configurado.
/// </summary>
public sealed class RunDeploymentHandler : ICommandHandler<RunDeploymentCommand, RunDeploymentResponse>
{
	#region Fields

	private readonly DeploymentOrchestratorService _service;

	#endregion

	#region Constructors

	/// <summary>
	/// Inicializa o handler de execução de deployment.
	/// </summary>
	public RunDeploymentHandler(DeploymentOrchestratorService service)
	{
		_service = service;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Executa o deployment solicitado.
	/// </summary>
	public RunDeploymentResponse Handle(RunDeploymentCommand command)
	{
		var selected = _service.FindDeploymentById(command.Id);
		if (selected is null)
		{
			return new RunDeploymentResponse(Found: false, Started: false);
		}

		_service.RunDeployment(selected);
		return new RunDeploymentResponse(Found: true, Started: true);
	}

	#endregion
}
