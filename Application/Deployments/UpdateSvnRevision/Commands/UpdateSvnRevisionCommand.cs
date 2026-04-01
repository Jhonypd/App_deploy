using App.Application.Messaging;
using App.Application.Deployments.UpdateSvnRevision.Responses;

namespace App.Application.Deployments.UpdateSvnRevision.Commands;

#region Command

/// <summary>
/// Comando para atualizar um deployment para uma revisão SVN específica.
/// </summary>
public sealed record UpdateSvnRevisionCommand(string Id, long Revision) : ICommand<UpdateSvnRevisionResponse>;

#endregion
