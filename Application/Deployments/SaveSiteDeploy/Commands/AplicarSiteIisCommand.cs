using App.Application.Messaging;
using System;
using System.Threading.Tasks;

namespace App.Application.Deployments.SaveSiteDeploy.Commands;

public class AplicarSiteIisCommand : ICommand<Task<bool>>
{
    public Guid Id { get; }

    public AplicarSiteIisCommand(Guid id)
    {
        Id = id;
    }
}
