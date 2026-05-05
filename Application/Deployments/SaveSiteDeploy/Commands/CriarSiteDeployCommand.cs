using App.Application.Messaging;
using App.Application.Deployments.SaveSiteDeploy.Requests;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using System.Threading.Tasks;

namespace App.Application.Deployments.SaveSiteDeploy.Commands;

public class CriarSiteDeployCommand : ICommand<Task<SiteDeployResponse>>
{
    public SalvarSiteDeployRequest Request { get; }

    public CriarSiteDeployCommand(SalvarSiteDeployRequest request)
    {
        Request = request;
    }
}
