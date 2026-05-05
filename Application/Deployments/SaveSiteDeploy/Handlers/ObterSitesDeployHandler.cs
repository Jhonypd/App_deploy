using App.Application.Messaging;
using App.Domain.Interfaces;
using App.Application.Deployments.SaveSiteDeploy.Queries;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace App.Application.Deployments.SaveSiteDeploy.Handlers;

public class ObterSitesDeployHandler : IQueryHandler<ObterSitesDeployQuery, Task<List<SiteDeployResponse>>>
{
    private readonly ISiteDeployRepository _repository;

    public ObterSitesDeployHandler(ISiteDeployRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SiteDeployResponse>> Handle(ObterSitesDeployQuery query)
    {
        var items = await _repository.ObterTodos();
        return items.Select(e => new SiteDeployResponse
        {
            Id = Guid.Parse(e.Id),
            ProjetoIis = e.ProjetoIis,
            Svn = e.Svn,
            Destino = e.Destino,
            UrlManual = e.UrlManual,
            Atualizada = e.Atualizada,
            Origens = e.Origens.Select(o => new SiteOrigemResponse
            {
                Id = Guid.Parse(o.Id),
                Path = o.Path,
                Conteudo = o.Conteudo
            }).ToList().AsReadOnly()
        }).ToList();
    }
}
