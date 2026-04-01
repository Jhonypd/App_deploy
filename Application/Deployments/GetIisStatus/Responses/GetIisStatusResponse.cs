namespace App.Application.Deployments.GetIisStatus;

#region Response Models

/// <summary>
/// Resposta da consulta de status IIS.
/// </summary>
public sealed record GetIisStatusResponse(IReadOnlyList<SiteStatus> Items);

#endregion
