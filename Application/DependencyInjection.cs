using App.Application.Messaging;
using App.Application.Interfaces;
using App.Application.Deployments.GetIisStatus;
using App.Application.Deployments.GetSvnCommits;
using App.Application.Deployments.GetAllDeployments;
using App.Application.Deployments.RunDeployment;
using App.Application.Deployments.UpdateSvnRevision;
using Microsoft.Extensions.DependencyInjection;

namespace App.Application;

/// <summary>
/// Extensões para registro dos serviços da camada de aplicação.
/// </summary>
public static class DependencyInjection
{
	#region Public Methods

	/// <summary>
	/// Registra serviços, comandos e consultas da aplicação no contêiner DI.
	/// </summary>
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddSingleton<IDeploymentConfigurationValidator, DeploymentConfigurationValidator>();
		services.AddSingleton<DeploymentOrchestratorService>();

		services.AddSingleton<IQueryHandler<GetAllDeploymentsQuery, GetAllDeploymentsResponse>, GetAllDeploymentsHandler>();
		services.AddSingleton<ICommandHandler<RunDeploymentCommand, RunDeploymentResponse>, RunDeploymentHandler>();
		services.AddSingleton<IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse>, GetSvnCommitsHandler>();
		services.AddSingleton<IQueryHandler<GetIisStatusQuery, GetIisStatusResponse>, GetIisStatusHandler>();
		services.AddSingleton<ICommandHandler<UpdateSvnRevisionCommand, UpdateSvnRevisionResponse>, UpdateSvnRevisionHandler>();

		return services;
	}

	#endregion
}
