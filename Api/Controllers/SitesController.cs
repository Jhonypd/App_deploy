using App.Application.Messaging;
using App.Application.Deployments.GetIisStatus.Queries;
using App.Application.Deployments.GetIisStatus.Responses;
using App.Application.Deployments.GetSvnCommits.Queries;
using App.Application.Deployments.GetSvnCommits.Responses;
using App.Application.Deployments.RunDeployment.Commands;
using App.Application.Deployments.RunDeployment.Responses;
using App.Application.Deployments.SaveSiteDeploy.Queries;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using App.Application.Deployments.UpdateSvnRevision.Commands;
using App.Application.Deployments.UpdateSvnRevision.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Api.Dtos.Deployments.Responses;

namespace App.Api.Controllers;

/// <summary>
/// Controlador responsável por listar deployments, executar deploy e consultar status do IIS e SVN.
/// </summary>
[ApiController]
[Route("v1/[controller]")]
public sealed class SitesController : ControllerBase
{
    #region Fields
    private readonly IQueryHandler<ObterSitesDeployQuery, Task<List<SiteDeployResponse>>> _obterSitesDeploy;
    private readonly ICommandHandler<RunDeploymentCommand, RunDeploymentResponse> _runDeployment;
    private readonly IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse> _svnCommits;
    private readonly IQueryHandler<GetIisStatusQuery, GetIisStatusResponse> _iisStatus;
    private readonly ICommandHandler<UpdateSvnRevisionCommand, UpdateSvnRevisionResponse> _svnUpdate;
    #endregion

    #region Constructors
    /// <summary>
    /// Inicializa as dependências de casos de uso do controlador.
    /// </summary>
    public SitesController(
        IQueryHandler<ObterSitesDeployQuery, Task<List<SiteDeployResponse>>> obterSitesDeploy,
        ICommandHandler<RunDeploymentCommand, RunDeploymentResponse> runDeployment,
        IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse> svnCommits,
        IQueryHandler<GetIisStatusQuery, GetIisStatusResponse> iisStatus,
        ICommandHandler<UpdateSvnRevisionCommand, UpdateSvnRevisionResponse> svnUpdate)
    {
        _obterSitesDeploy = obterSitesDeploy;
        _runDeployment = runDeployment;
        _svnCommits = svnCommits;
        _iisStatus = iisStatus;
        _svnUpdate = svnUpdate;
    }
    #endregion

    #region Endpoints
    /// <summary>
    /// Lista os sites cadastrados, cruzando com o que existe no IIS.
    /// </summary>
    [HttpGet("listar")]
    public async Task<IActionResult> GetAllDeployments(
        [FromQuery(Name = "importadoIis")] bool? importadoIis = null,
        [FromQuery(Name = "atualizada")] bool? atualizada = null)
    {
        var response = await _obterSitesDeploy.Handle(new ObterSitesDeployQuery());
        var iisSiteNames = _iisStatus.Handle(new GetIisStatusQuery()).Status.Sites
            .Select(s => NormalizeName(s.Nome))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var list = response
            .Select(d => new SiteListItemDto
            {
                Id = d.Id,
                ProjetoIis = d.ProjetoIis,
                Porta = d.Porta,
                Svn = d.Svn,
                Destino = d.Destino,
                UrlManual = d.UrlManual,
                Atualizada = d.Atualizada,
                ImportadoIis = iisSiteNames.Contains(NormalizeName(d.ProjetoIis)),
                Origens = d.Origens.Select(o => new SiteListOriginDto
                {
                    Id = o.Id,
                    Path = o.Path,
                    Conteudo = o.Conteudo
                }).ToList()
            })
            .Where(item => !importadoIis.HasValue || item.ImportadoIis == importadoIis.Value)
            .Where(item => !atualizada.HasValue || item.Atualizada == atualizada.Value)
            .ToList();

        return ApiResponseFactory.Ok(HttpContext, list);
    }

    /// <summary>
    /// Executa o deployment da aplicação informada pelo identificador.
    /// </summary>
    [HttpPost("executar")]
    public IActionResult RunDeployment([FromHeader(Name = "id")] string? id)
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

    /// <summary>
    /// Lista os commits SVN mais recentes do deployment informado.
    /// </summary>
    [HttpGet("svn/commits")]
    public IActionResult SvnCommits(
        [FromHeader(Name = "id")] string? id,
        [FromQuery(Name = "limit")] int limit = 20)
    {
        return HandleSvnCommitsRequest(id, limit);
    }

    /// <summary>
    /// Atualiza o working copy SVN do deployment para uma revisão específica.
    /// </summary>
    [HttpPost("svn/voltar-commit")]
    public IActionResult SvnUpdate(
        [FromHeader(Name = "id")] string? id,
        [FromHeader(Name = "revision")] long revision)
    {
        return HandleSvnUpdateRequest(id, revision);
    }

    /// <summary>
    /// Retorna o status atual dos sites no IIS.
    /// </summary>
    [HttpGet("iis/status")]
    public IActionResult GetIisStatus()
    {
        var response = _iisStatus.Handle(new GetIisStatusQuery());

        var statusDto = new
        {
            Sites = response.Status.Sites.Select(s => new SiteStatusDto
            {
                Nome = s.Nome,
                EstadoSite = s.EstadoSite,
                AppPool = s.AppPool,
                EstadoAppPool = s.EstadoAppPool,
                Pid = s.Pid,
                MemoriaMb = s.MemoriaMb,
                Cpu = s.Cpu,
                ProcessoAtivo = s.ProcessoAtivo
            }).ToList(),
            TotalMemoriaMb = response.Status.TotalMemoriaMb,
            TotalCpu = response.Status.TotalCpu,
            TotalSites = response.Status.TotalSites,
            TotalSitesComProcessoAtivo = response.Status.TotalSitesComProcessoAtivo
        };

        return ApiResponseFactory.Ok(HttpContext, statusDto);
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Valida e processa a consulta de commits SVN.
    /// </summary>
    private IActionResult HandleSvnCommitsRequest(string? id, int limit)
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

    /// <summary>
    /// Valida e processa a atualização SVN para uma revisão específica.
    /// </summary>
    private IActionResult HandleSvnUpdateRequest(string? id, long revision)
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

    /// <summary>
    /// Normaliza o nome do site para comparação com o IIS.
    /// </summary>
    private static string NormalizeName(string? name)
    {
        return string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();
    }
    #endregion
}
