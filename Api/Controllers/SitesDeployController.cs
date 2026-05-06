using App.Application.Messaging;
using App.Application.Deployments.SaveSiteDeploy.Commands;
using App.Application.Deployments.SaveSiteDeploy.Queries;
using App.Application.Deployments.SaveSiteDeploy.Requests;
using App.Application.Deployments.SaveSiteDeploy.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Api;

namespace App.Api.Controllers;

/// <summary>
/// Controlador responsável pelo CRUD de configurações de sites de deploy no SQLite.
/// </summary>
[ApiController]
[Route("api/sites-deploy")]
public sealed class SitesDeployController : ControllerBase
{
    private readonly IQueryHandler<ObterSitesDeployQuery, Task<List<SiteDeployResponse>>> _obterTodos;
    private readonly IQueryHandler<ObterSiteDeployPorIdQuery, Task<SiteDeployResponse?>> _obterPorId;
    private readonly ICommandHandler<CriarSiteDeployCommand, Task<SiteDeployResponse>> _criar;
    private readonly ICommandHandler<AtualizarSiteDeployCommand, Task<SiteDeployResponse>> _atualizar;
    private readonly ICommandHandler<RemoverSiteDeployCommand, Task<bool>> _remover;
    private readonly ICommandHandler<AplicarSiteIisCommand, Task<bool>> _aplicarIis;
    private readonly ICommandHandler<RemoverSiteIisCommand, Task<bool>> _removerIis;

    public SitesDeployController(
        IQueryHandler<ObterSitesDeployQuery, Task<List<SiteDeployResponse>>> obterTodos,
        IQueryHandler<ObterSiteDeployPorIdQuery, Task<SiteDeployResponse?>> obterPorId,
        ICommandHandler<CriarSiteDeployCommand, Task<SiteDeployResponse>> criar,
        ICommandHandler<AtualizarSiteDeployCommand, Task<SiteDeployResponse>> atualizar,
        ICommandHandler<RemoverSiteDeployCommand, Task<bool>> remover,
        ICommandHandler<AplicarSiteIisCommand, Task<bool>> aplicarIis,
        ICommandHandler<RemoverSiteIisCommand, Task<bool>> removerIis)
    {
        _obterTodos = obterTodos;
        _obterPorId = obterPorId;
        _criar = criar;
        _atualizar = atualizar;
        _remover = remover;
        _aplicarIis = aplicarIis;
        _removerIis = removerIis;
    }

    /// <summary>
    /// Lista todas as configurações de sites de deploy.
    /// </summary>
    [HttpGet("listar")]
    public async Task<IActionResult> Get()
    {
        var response = await _obterTodos.Handle(new ObterSitesDeployQuery());
        return ApiResponseFactory.Ok(HttpContext, response);
    }

    /// <summary>
    /// Obtém uma configuração de site de deploy pelo identificador Guid.
    /// </summary>
    [HttpGet("obter/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _obterPorId.Handle(new ObterSiteDeployPorIdQuery(id));
        if (response == null) return NotFound(ApiResponseFactory.Error(HttpContext, 404, "Site não encontrado."));

        return ApiResponseFactory.Ok(HttpContext, response);
    }

    /// <summary>
    /// Insere uma nova configuração de site de deploy.
    /// </summary>
    [HttpPost("inserir")]
    public async Task<IActionResult> Post([FromBody] SalvarSiteDeployRequest request)
    {
        try
        {
            var response = await _criar.Handle(new CriarSiteDeployCommand(request));
            return ApiResponseFactory.Ok(HttpContext, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponseFactory.Error(HttpContext, 400, ex.Message));
        }
    }

    /// <summary>
    /// Atualiza uma configuração de site de deploy existente.
    /// </summary>
    [HttpPut("atualizar/{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] SalvarSiteDeployRequest request)
    {
        try
        {
            var response = await _atualizar.Handle(new AtualizarSiteDeployCommand(id, request));
            return ApiResponseFactory.Ok(HttpContext, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponseFactory.Error(HttpContext, 400, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponseFactory.Error(HttpContext, 404, ex.Message));
        }
    }

    /// <summary>
    /// Remove uma configuração de site de deploy.
    /// </summary>
    [HttpDelete("deletar/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _remover.Handle(new RemoverSiteDeployCommand(id));
        return ApiResponseFactory.Ok(HttpContext, new { success = true });
    }

    /// <summary>
    /// Aplica a configuração cadastrada no IIS.
    /// </summary>
    [HttpPost("{id:guid}/aplicar-iis")]
    public async Task<IActionResult> AplicarIis(Guid id)
    {
        try
        {
            await _aplicarIis.Handle(new AplicarSiteIisCommand(id));
            return ApiResponseFactory.Ok(HttpContext, new { success = true, message = "Site aplicado no IIS." });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponseFactory.Error(HttpContext, 404, ex.Message));
        }
    }

    /// <summary>
    /// Remove a configuração cadastrada do IIS.
    /// </summary>
    [HttpPost("{id:guid}/remover-iis")]
    public async Task<IActionResult> RemoverIis(Guid id)
    {
        try
        {
            await _removerIis.Handle(new RemoverSiteIisCommand(id));
            return ApiResponseFactory.Ok(HttpContext, new { success = true, message = "Site removido do IIS." });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponseFactory.Error(HttpContext, 404, ex.Message));
        }
    }
}
