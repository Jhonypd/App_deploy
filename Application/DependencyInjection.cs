using App.Application.Messaging;
using App.Application.Interfaces;
using App.Application.Deployments.GetAllDeployments.Handlers;
using App.Application.Deployments.GetAllDeployments.Queries;
using App.Application.Deployments.GetAllDeployments.Responses;
using App.Application.Deployments.GetIisStatus.Handlers;
using App.Application.Deployments.GetIisStatus.Queries;
using App.Application.Deployments.GetIisStatus.Responses;
using App.Application.Deployments.GetSvnCommits.Handlers;
using App.Application.Deployments.GetSvnCommits.Queries;
using App.Application.Deployments.GetSvnCommits.Responses;
using App.Application.Deployments.RunDeployment.Commands;
using App.Application.Deployments.RunDeployment.Handlers;
using App.Application.Deployments.RunDeployment.Responses;
using App.Application.Deployments.UpdateSvnRevision.Commands;
using App.Application.Deployments.UpdateSvnRevision.Handlers;
using App.Application.Deployments.UpdateSvnRevision.Responses;
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
