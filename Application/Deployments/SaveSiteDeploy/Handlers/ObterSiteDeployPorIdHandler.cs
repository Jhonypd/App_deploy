using App.Application.Messaging;
using App.Domain.Interfaces;
using App.Application.Deployments.SaveSiteDeploy.Queries;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace App.Application.Deployments.SaveSiteDeploy.Handlers;

public class ObterSiteDeployPorIdHandler : IQueryHandler<ObterSiteDeployPorIdQuery, Task<SiteDeployResponse?>>
{
    private readonly ISiteDeployRepository _repository;

    public ObterSiteDeployPorIdHandler(ISiteDeployRepository repository)
    {
        _repository = repository;
    }

    public async Task<SiteDeployResponse?> Handle(ObterSiteDeployPorIdQuery query)
    {
        var e = await _repository.ObterPorId(query.Id);
        if (e == null) return null;

        return new SiteDeployResponse
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
        };
    }
}
