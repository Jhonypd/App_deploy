namespace App.Application.UseCases.GetIisStatus;

public sealed record GetIisStatusResponse(IReadOnlyList<SiteStatus> Items);
