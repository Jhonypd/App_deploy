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
using App.Application.Deployments.SaveSiteDeploy.Commands;
using App.Application.Deployments.SaveSiteDeploy.Queries;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using App.Application.Deployments.SaveSiteDeploy.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

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
		services.AddScoped<IDeploymentConfigurationValidator, DeploymentConfigurationValidator>();
		services.AddScoped<DeploymentOrchestratorService>();

		services.AddScoped<IQueryHandler<GetAllDeploymentsQuery, GetAllDeploymentsResponse>, GetAllDeploymentsHandler>();
		services.AddScoped<ICommandHandler<RunDeploymentCommand, RunDeploymentResponse>, RunDeploymentHandler>();
		services.AddScoped<IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse>, GetSvnCommitsHandler>();
		services.AddScoped<IQueryHandler<GetIisStatusQuery, GetIisStatusResponse>, GetIisStatusHandler>();
		services.AddScoped<ICommandHandler<UpdateSvnRevisionCommand, UpdateSvnRevisionResponse>, UpdateSvnRevisionHandler>();

		services.AddScoped<ICommandHandler<CriarSiteDeployCommand, Task<SiteDeployResponse>>, CriarSiteDeployHandler>();
		services.AddScoped<ICommandHandler<AtualizarSiteDeployCommand, Task<SiteDeployResponse>>, AtualizarSiteDeployHandler>();
		services.AddScoped<ICommandHandler<RemoverSiteDeployCommand, Task<bool>>, RemoverSiteDeployHandler>();
		services.AddScoped<IQueryHandler<ObterSitesDeployQuery, Task<List<SiteDeployResponse>>>, ObterSitesDeployHandler>();
		services.AddScoped<IQueryHandler<ObterSiteDeployPorIdQuery, Task<SiteDeployResponse?>>, ObterSiteDeployPorIdHandler>();

		services.AddScoped<ICommandHandler<AplicarSiteIisCommand, Task<bool>>, AplicarSiteIisHandler>();
		services.AddScoped<ICommandHandler<RemoverSiteIisCommand, Task<bool>>, RemoverSiteIisHandler>();

		return services;
	}

	#endregion
}
