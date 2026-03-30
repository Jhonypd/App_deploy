using App.Application.Abstractions;
using App.Application.Ports;
using App.Application.UseCases.GetIisStatus;
using App.Application.UseCases.GetSvnCommits;
using App.Application.UseCases.ListDeployments;
using App.Application.UseCases.RunDeployment;
using Microsoft.Extensions.DependencyInjection;

namespace App.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddSingleton<IDeploymentValidator, DeploymentValidator>();
		services.AddSingleton<DeploymentService>();

		services.AddSingleton<IQueryHandler<ListDeploymentsQuery, ListDeploymentsResponse>, ListDeploymentsHandler>();
		services.AddSingleton<ICommandHandler<RunDeploymentCommand, RunDeploymentResponse>, RunDeploymentHandler>();
		services.AddSingleton<IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse>, GetSvnCommitsHandler>();
		services.AddSingleton<IQueryHandler<GetIisStatusQuery, GetIisStatusResponse>, GetIisStatusHandler>();

		return services;
	}
}
