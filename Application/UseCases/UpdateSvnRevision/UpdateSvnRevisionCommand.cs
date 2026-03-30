using App.Application.Abstractions;

namespace App.Application.UseCases.UpdateSvnRevision;

public sealed record UpdateSvnRevisionCommand(string Id, long Revision) : ICommand<UpdateSvnRevisionResponse>;
