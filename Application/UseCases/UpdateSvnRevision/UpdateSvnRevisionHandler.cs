using App.Application.Abstractions;

namespace App.Application.UseCases.UpdateSvnRevision;

public sealed class UpdateSvnRevisionHandler : ICommandHandler<UpdateSvnRevisionCommand, UpdateSvnRevisionResponse>
{
    private readonly DeploymentService _service;

    public UpdateSvnRevisionHandler(DeploymentService service)
    {
        _service = service;
    }

    public UpdateSvnRevisionResponse Handle(UpdateSvnRevisionCommand command)
    {
        var selected = _service.FindById(command.Id);
        if (selected is null)
        {
            return new UpdateSvnRevisionResponse(Found: false, Updated: false, SvnPath: string.Empty, Revision: command.Revision);
        }

        _service.UpdateSvnToRevision(selected, command.Revision);
        return new UpdateSvnRevisionResponse(Found: true, Updated: true, SvnPath: selected.Svn, Revision: command.Revision);
    }
}
