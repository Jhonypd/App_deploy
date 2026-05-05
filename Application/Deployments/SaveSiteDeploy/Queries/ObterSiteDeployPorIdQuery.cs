using App.Application.Messaging;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using System;
using System.Threading.Tasks;

namespace App.Application.Deployments.SaveSiteDeploy.Queries;

public class ObterSiteDeployPorIdQuery : IQuery<Task<SiteDeployResponse?>>
{
    public Guid Id { get; }

    public ObterSiteDeployPorIdQuery(Guid id)
    {
        Id = id;
    }
}
