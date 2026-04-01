using App.Application.Messaging;
using App.Application.Deployments.GetIisStatus;
using App.Application.Deployments.GetSvnCommits;
using App.Application.Deployments.GetAllDeployments;
using App.Application.Deployments.RunDeployment;
using App.Application.Deployments.UpdateSvnRevision;
using App.Api.Dtos.Deployments.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("v1/[controller]")]
/// <summary>
/// Expõe endpoints para listagem, execução e status de deployments e SVN.
/// </summary>
public sealed class SitesController : ControllerBase
{
    #region Fields
    private readonly IQueryHandler<GetAllDeploymentsQuery, GetAllDeploymentsResponse> _getAllDeployments;
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
        IQueryHandler<GetAllDeploymentsQuery, GetAllDeploymentsResponse> getAllDeployments,
        ICommandHandler<RunDeploymentCommand, RunDeploymentResponse> runDeployment,
        IQueryHandler<GetSvnCommitsQuery, GetSvnCommitsResponse> svnCommits,
        IQueryHandler<GetIisStatusQuery, GetIisStatusResponse> iisStatus,
        ICommandHandler<UpdateSvnRevisionCommand, UpdateSvnRevisionResponse> svnUpdate)
    {
        _getAllDeployments = getAllDeployments;
        _runDeployment = runDeployment;
        _svnCommits = svnCommits;
        _iisStatus = iisStatus;
        _svnUpdate = svnUpdate;
    }
    #endregion

    #region Endpoints
    /// <summary>
    /// Retorna todos os deployments configurados.
    /// </summary>
    [HttpGet("Lista")]
    public IActionResult GetAllDeployments()
    {
        var response = _getAllDeployments.Handle(new GetAllDeploymentsQuery());

        var list = response.Items
            .Select(d => new DeploymentDto
            {
                Id = d.Id,
                NomeSite = d.NomeSite,
                Svn = d.Svn,
                Destino = d.Destino,
                Origins = d.Origins.Select(o => new OriginDto { Path = o.Path, Conteudo = o.Conteudo }).ToList()
            })
            .ToList();

        return ApiResponseFactory.Ok(HttpContext, list);
    }

    /// <summary>
    /// Executa o deployment do item informado.
    /// </summary>
    [HttpPost("Executar")]
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
    /// Lista os commits SVN mais recentes para o item selecionado.
    /// </summary>
    [HttpGet("SvnCommits")]
    public IActionResult SvnCommits(
        [FromHeader(Name = "id")] string? id,
        [FromQuery(Name = "limit")] int limit = 20)
    {
        return HandleSvnCommitsRequest(id, limit);
    }

    /// <summary>
    /// Atualiza o working copy para uma revisão específica do SVN.
    /// </summary>
    [HttpPost("SvnUpdate")]
    public IActionResult SvnUpdate(
        [FromHeader(Name = "id")] string? id,
        [FromHeader(Name = "revision")] long revision)
    {
        return HandleSvnUpdateRequest(id, revision);
    }

    /// <summary>
    /// Retorna o status atual dos sites no IIS.
    /// </summary>
    [HttpGet("Status")]
    public IActionResult GetIisStatus()
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
    #endregion
}
