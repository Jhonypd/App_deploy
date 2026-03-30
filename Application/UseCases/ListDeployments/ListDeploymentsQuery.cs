using App.Application.Abstractions;

namespace App.Application.UseCases.ListDeployments;

public sealed record ListDeploymentsQuery() : IQuery<ListDeploymentsResponse>;
