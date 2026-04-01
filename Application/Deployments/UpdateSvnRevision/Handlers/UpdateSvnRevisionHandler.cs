using App.Application.Messaging;

namespace App.Application.Deployments.UpdateSvnRevision;

/// <summary>
/// Manipula a atualização de revisão SVN para um deployment.
/// </summary>
public sealed class UpdateSvnRevisionHandler : ICommandHandler<UpdateSvnRevisionCommand, UpdateSvnRevisionResponse>
{
    #region Fields

    private readonly DeploymentOrchestratorService _service;

    #endregion

    #region Constructors

    /// <summary>
    /// Inicializa o handler de atualização de revisão SVN.
    /// </summary>
    public UpdateSvnRevisionHandler(DeploymentOrchestratorService service)
    {
        _service = service;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Atualiza o deployment para a revisão informada.
    /// </summary>
    public UpdateSvnRevisionResponse Handle(UpdateSvnRevisionCommand command)
    {
        var selected = _service.FindDeploymentById(command.Id);
        if (selected is null)
        {
            return new UpdateSvnRevisionResponse(Found: false, Updated: false, SvnPath: string.Empty, Revision: command.Revision);
        }

        _service.RunDeploymentAtRevision(selected, command.Revision);
        return new UpdateSvnRevisionResponse(Found: true, Updated: true, SvnPath: selected.Svn, Revision: command.Revision);
    }

    #endregion
}
