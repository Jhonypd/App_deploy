using App.Application.Messaging;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Application.Deployments.SaveSiteDeploy.Queries;

public class ObterSitesDeployQuery : IQuery<Task<List<SiteDeployResponse>>>
{
}
