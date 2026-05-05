using App.Application.Messaging;
using App.Application.Interfaces;
using App.Domain.Interfaces;
using App.Application.Deployments.SaveSiteDeploy.Commands;
using System.Threading.Tasks;
using System;

namespace App.Application.Deployments.SaveSiteDeploy.Handlers;

public class RemoverSiteIisHandler : ICommandHandler<RemoverSiteIisCommand, Task<bool>>
{
    private readonly ISiteDeployRepository _repository;
    private readonly IIisSiteManager _iisManager;

    public RemoverSiteIisHandler(ISiteDeployRepository repository, IIisSiteManager iisManager)
    {
        _repository = repository;
        _iisManager = iisManager;
    }

    public async Task<bool> Handle(RemoverSiteIisCommand command)
    {
        var config = await _repository.ObterPorId(command.Id);
        if (config == null) throw new InvalidOperationException("SiteDeployConfig não encontrado.");

        _iisManager.RemoverSite(config.ProjetoIis);

        return true;
    }
}
