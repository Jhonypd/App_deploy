using App.Application.Messaging;
using App.Domain.Entities;
using App.Domain.Interfaces;
using App.Application.Deployments.SaveSiteDeploy.Commands;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace App.Application.Deployments.SaveSiteDeploy.Handlers;

public class AtualizarSiteDeployHandler : ICommandHandler<AtualizarSiteDeployCommand, Task<SiteDeployResponse>>
{
    private readonly ISiteDeployRepository _repository;

    public AtualizarSiteDeployHandler(ISiteDeployRepository repository)
    {
        _repository = repository;
    }

    public async Task<SiteDeployResponse> Handle(AtualizarSiteDeployCommand command)
    {
        var req = command.Request;

        ValidarRequest(req);

        var entity = await _repository.ObterPorId(command.Id);
        if (entity == null) throw new InvalidOperationException("SiteDeployConfig não encontrado.");

        entity.ProjetoIis = req.ProjetoIis;
        entity.Porta = req.Porta;
        entity.Svn = req.Svn;
        entity.Destino = req.Destino;
        entity.UrlManual = req.UrlManual;
        entity.Atualizada = req.Atualizada;

        entity.Origens.Clear();
        foreach (var origem in req.Origens)
        {
            entity.Origens.Add(new SiteOrigemConfig
            {
                Id = Guid.NewGuid().ToString("D"),
                SiteDeployConfigId = entity.Id,
                Path = origem.Path,
                Conteudo = origem.Conteudo
            });
        }

        await _repository.Atualizar(entity);

        return new SiteDeployResponse
        {
            Id = Guid.Parse(entity.Id),
            ProjetoIis = entity.ProjetoIis,
            Porta = entity.Porta,
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
