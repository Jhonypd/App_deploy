using App.Application.Messaging;
using App.Domain.Interfaces;
using App.Application.Deployments.SaveSiteDeploy.Commands;
using System.Threading.Tasks;

namespace App.Application.Deployments.SaveSiteDeploy.Handlers;

public class RemoverSiteDeployHandler : ICommandHandler<RemoverSiteDeployCommand, Task<bool>>
{
    private readonly ISiteDeployRepository _repository;

    public RemoverSiteDeployHandler(ISiteDeployRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(RemoverSiteDeployCommand command)
    {
        await _repository.Remover(command.Id);
        return true;
    }
}
