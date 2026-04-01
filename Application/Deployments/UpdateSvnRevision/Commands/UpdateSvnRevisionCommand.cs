using App.Application.Messaging;

namespace App.Application.Deployments.UpdateSvnRevision;

#region Command

/// <summary>
/// Comando para atualizar um deployment para uma revisão SVN específica.
/// </summary>
public sealed record UpdateSvnRevisionCommand(string Id, long Revision) : ICommand<UpdateSvnRevisionResponse>;

#endregion
