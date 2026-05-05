using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Domain.Entities;

namespace App.Domain.Interfaces;

public interface ISiteDeployRepository
{
    Task<List<SiteDeployConfig>> ObterTodos();
    Task<SiteDeployConfig?> ObterPorId(Guid id);
    Task Inserir(SiteDeployConfig entity);
    Task Atualizar(SiteDeployConfig entity);
    Task Remover(Guid id);
}
