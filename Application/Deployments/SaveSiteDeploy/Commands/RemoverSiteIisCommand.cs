using App.Application.Messaging;
using System;
using System.Threading.Tasks;

namespace App.Application.Deployments.SaveSiteDeploy.Commands;

public class RemoverSiteIisCommand : ICommand<Task<bool>>
{
    public Guid Id { get; }

    public RemoverSiteIisCommand(Guid id)
    {
        Id = id;
    }
}
