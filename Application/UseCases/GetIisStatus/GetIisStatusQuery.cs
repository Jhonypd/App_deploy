using App.Application.Abstractions;

namespace App.Application.UseCases.GetIisStatus;

public sealed record GetIisStatusQuery() : IQuery<GetIisStatusResponse>;
