using App.Application.Messaging;
using App.Application.Deployments.SaveSiteDeploy.Requests;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using System;
using System.Threading.Tasks;

namespace App.Application.Deployments.SaveSiteDeploy.Commands;

public class AtualizarSiteDeployCommand : ICommand<Task<SiteDeployResponse>>
{
    public Guid Id { get; }
    public SalvarSiteDeployRequest Request { get; }

    public AtualizarSiteDeployCommand(Guid id, SalvarSiteDeployRequest request)
    {
        Id = id;
        Request = request;
    }
}
