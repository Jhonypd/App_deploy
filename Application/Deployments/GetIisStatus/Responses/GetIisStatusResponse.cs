using App.Application;

namespace App.Application.Deployments.GetIisStatus.Responses;

#region Response Models

/// <summary>
/// Resposta da consulta de status IIS.
/// </summary>
public sealed record GetIisStatusResponse(IReadOnlyList<SiteStatus> Items);

#endregion
