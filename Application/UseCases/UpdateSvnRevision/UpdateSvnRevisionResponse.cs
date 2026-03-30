namespace App.Application.UseCases.UpdateSvnRevision;

public sealed record UpdateSvnRevisionResponse(bool Found, bool Updated, string SvnPath, long Revision);
