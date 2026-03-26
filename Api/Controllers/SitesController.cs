using App.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("v1/[controller]")]
public sealed class SitesController : ControllerBase
{
    private readonly DeploymentService _deployments;

    public SitesController(DeploymentService deployments)
    {
        _deployments = deployments;
    }

    [HttpGet("Lista")]
    public IActionResult List()
    {
        var list = _deployments.ListDeployments()
            .Select(d => new DeploymentDto
            {
                Id = d.Id,
                NomeSite = d.NomeSite,
                Svn = d.Svn,
                Destino = d.Destino,
                Origens = d.Origens.ToList()
            })
            .ToList();

        return Ok(list);
    }

    [HttpPost("Executar")]
    public IActionResult Run([FromHeader(Name = "id")] string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "O id é obrigatório.");
        }

        var selected = _deployments.FindById(id);
        if (selected is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: $"Não foi encontrado um serviço com o id {id}.");
        }

        _deployments.Run(selected);
        return Ok(new { ok = true });
    }

    [HttpGet("Status")]
    public IActionResult Status([FromServices] IisMonitorService monitor)
    {
        var status = monitor.GetStatus();
        return Ok(status);
    }
}
