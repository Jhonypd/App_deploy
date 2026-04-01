namespace App.Application.Deployments.UpdateSvnRevision.Responses;

#region Response Models

/// <summary>
/// Resposta da atualização de revisão SVN.
/// </summary>
public sealed record UpdateSvnRevisionResponse(bool Found, bool Updated, string SvnPath, long Revision);

#endregion
