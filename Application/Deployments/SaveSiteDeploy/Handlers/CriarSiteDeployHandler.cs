using App.Application.Messaging;
using App.Domain.Entities;
using App.Domain.Interfaces;
using App.Application.Deployments.SaveSiteDeploy.Commands;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace App.Application.Deployments.SaveSiteDeploy.Handlers;

public class CriarSiteDeployHandler : ICommandHandler<CriarSiteDeployCommand, Task<SiteDeployResponse>>
{
    private readonly ISiteDeployRepository _repository;

    public CriarSiteDeployHandler(ISiteDeployRepository repository)
    {
        _repository = repository;
    }

    public async Task<SiteDeployResponse> Handle(CriarSiteDeployCommand command)
    {
        var req = command.Request;

        ValidarRequest(req);

        var entityId = Guid.NewGuid().ToString("D");
        var entity = new SiteDeployConfig
        {
            Id = entityId,
            ProjetoIis = req.ProjetoIis,
            Svn = req.Svn,
            Destino = req.Destino,
            UrlManual = req.UrlManual,
            Atualizada = req.Atualizada,
            Origens = req.Origens.Select(o => new SiteOrigemConfig
            {
                Id = Guid.NewGuid().ToString("D"),
                SiteDeployConfigId = entityId,
                Path = o.Path,
                Conteudo = o.Conteudo
            }).ToList()
        };

        await _repository.Inserir(entity);

        return new SiteDeployResponse
        {
            Id = Guid.Parse(entity.Id),
            ProjetoIis = entity.ProjetoIis,
            Svn = entity.Svn,
            Destino = entity.Destino,
            UrlManual = entity.UrlManual,
            Atualizada = entity.Atualizada,
            Origens = entity.Origens.Select(o => new SiteOrigemResponse
            {
                Id = Guid.Parse(o.Id),
                Path = o.Path,
                Conteudo = o.Conteudo
            }).ToList().AsReadOnly()
        };
    }

    private static void ValidarRequest(App.Application.Deployments.SaveSiteDeploy.Requests.SalvarSiteDeployRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.ProjetoIis))
        {
            throw new ArgumentException("ProjetoIis é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(req.Destino))
        {
            throw new ArgumentException("Destino é obrigatório.");
        }

        if (req.Origens.Any(o => string.IsNullOrWhiteSpace(o.Path)))
        {
            throw new ArgumentException("Cada origem precisa ter Path obrigatório.");
        }
    }
}
