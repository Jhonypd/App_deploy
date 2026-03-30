using App.Application.Abstractions;
using App.Application.UseCases.GetIisStatus;
using App.Application.UseCases.GetSvnCommits;
using App.Application.UseCases.ListDeployments;
using App.Application.UseCases.RunDeployment;
using App.Application.UseCases.UpdateSvnRevision;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("v1/[controller]")]
public sealed class SitesController : ControllerBase
{
    private readonly IQueryHandler<ListDeploymentsQuery, ListDeploymentsResponse> _listDeployments;
    private readonly ICommandHandler<RunDeploymentCommand, RunDeploymentResponse> _runDeployment;
    private readonly IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse> _svnCommits;
    private readonly IQueryHandler<GetIisStatusQuery, GetIisStatusResponse> _iisStatus;
    private readonly ICommandHandler<UpdateSvnRevisionCommand, UpdateSvnRevisionResponse> _svnUpdate;

    public SitesController(
        IQueryHandler<ListDeploymentsQuery, ListDeploymentsResponse> listDeployments,
        ICommandHandler<RunDeploymentCommand, RunDeploymentResponse> runDeployment,
        IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse> svnCommits,
        IQueryHandler<GetIisStatusQuery, GetIisStatusResponse> iisStatus,
        ICommandHandler<UpdateSvnRevisionCommand, UpdateSvnRevisionResponse> svnUpdate)
    {
        _listDeployments = listDeployments;
        _runDeployment = runDeployment;
        _svnCommits = svnCommits;
        _iisStatus = iisStatus;
        _svnUpdate = svnUpdate;
    }

    [HttpGet("Lista")]
    public IActionResult List()
    {
        var response = _listDeployments.Handle(new ListDeploymentsQuery());

        var list = response.Items
            .Select(d => new DeploymentDto
            {
                Id = d.Id,
                NomeSite = d.NomeSite,
                Svn = d.Svn,
                Destino = d.Destino,
                Origens = d.Origens.Select(o => new OriginDto { Path = o.Path, Conteudo = o.Conteudo }).ToList()
            })
            .ToList();

        return ApiResponseFactory.Ok(HttpContext, list);
    }

    [HttpPost("Executar")]
    public IActionResult Run([FromHeader(Name = "id")] string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return ApiResponseFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "O id é obrigatório.");
        }

        var response = _runDeployment.Handle(new RunDeploymentCommand(id));
        if (!response.Found)
        {
            return ApiResponseFactory.Error(HttpContext, StatusCodes.Status404NotFound, $"Não foi encontrado um serviço com o id {id}.");
        }

        return ApiResponseFactory.Ok(HttpContext, new { ok = response.Started });
    }

    [HttpGet("SvnCommits")]
    public IActionResult SvnCommits(
        [FromHeader(Name = "id")] string? id,
        [FromQuery(Name = "limit")] int limit = 20)
    {
        return SvnCommitsInternal(id, limit);
    }

    [HttpPost("SvnUpdate")]
    public IActionResult SvnUpdate(
        [FromHeader(Name = "id")] string? id,
        [FromHeader(Name = "revision")] long revision)
    {
        return SvnUpdateInternal(id, revision);
    }


    private IActionResult SvnCommitsInternal(string? id, int limit)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return ApiResponseFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "O id é obrigatório.");
        }

        if (limit <= 0)
        {
            return ApiResponseFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "O limit deve ser maior que zero.");
        }

        var response = _svnCommits.Handle(new GetSvnCommitsQuery(id, limit));
        if (!response.Found)
        {
            return ApiResponseFactory.Error(HttpContext, StatusCodes.Status404NotFound, $"Não foi encontrado um serviço com o id {id}.");
        }

        var commits = response.Items
            .Select(c => new SvnCommitDto
            {
                Revision = c.Revision,
                Author = c.Author,
                Date = c.Date,
                Message = c.Message,
                IsCurrent = c.IsCurrent
            })
            .ToList();

        return ApiResponseFactory.Ok(HttpContext, commits);
    }

    private IActionResult SvnUpdateInternal(string? id, long revision)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return ApiResponseFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "O id é obrigatório.");
        }

        if (revision <= 0)
        {
            return ApiResponseFactory.Error(HttpContext, StatusCodes.Status400BadRequest, "A revision deve ser maior que zero.");
        }

        var response = _svnUpdate.Handle(new UpdateSvnRevisionCommand(id, revision));
        if (!response.Found)
        {
            return ApiResponseFactory.Error(HttpContext, StatusCodes.Status404NotFound, $"Não foi encontrado um serviço com o id {id}.");
        }

        return ApiResponseFactory.Ok(HttpContext, new { ok = response.Updated, id, revision, svn = response.SvnPath });
    }

    [HttpGet("Status")]
    public IActionResult Status()
    {
        var response = _iisStatus.Handle(new GetIisStatusQuery());

        var status = response.Items
            .Select(s => new SiteStatusDto
            {
                Nome = s.Nome,
                EstadoSite = s.EstadoSite,
                AppPool = s.AppPool,
                EstadoAppPool = s.EstadoAppPool,
                Pid = s.Pid,
                MemoriaMb = s.MemoriaMb,
                Cpu = s.Cpu,
                ProcessoAtivo = s.ProcessoAtivo
            })
            .ToList();

        return ApiResponseFactory.Ok(HttpContext, status);
    }
}
