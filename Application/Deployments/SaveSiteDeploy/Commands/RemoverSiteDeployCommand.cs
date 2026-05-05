using App.Application.Messaging;
using System;
using System.Threading.Tasks;

namespace App.Application.Deployments.SaveSiteDeploy.Commands;

public class RemoverSiteDeployCommand : ICommand<Task<bool>>
{
    public Guid Id { get; }

    public RemoverSiteDeployCommand(Guid id)
    {
        Id = id;
    }
}
