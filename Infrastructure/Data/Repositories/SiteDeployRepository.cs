using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Domain.Entities;
using App.Domain.Interfaces;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Data.Repositories;

public class SiteDeployRepository : ISiteDeployRepository
{
    private readonly AppDbContext _context;

    public SiteDeployRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<SiteDeployConfig>> ObterTodos()
    {
        return _context.SiteDeployConfigs
            .Include(x => x.Origens)
            .ToListAsync();
    }

    public Task<SiteDeployConfig?> ObterPorId(Guid id)
    {
        return _context.SiteDeployConfigs
            .Include(x => x.Origens)
            .FirstOrDefaultAsync(x => x.Id == id.ToString("D"));
    }

    public async Task Inserir(SiteDeployConfig entity)
    {
        _context.SiteDeployConfigs.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Atualizar(SiteDeployConfig entity)
    {
        _context.SiteDeployConfigs.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Remover(Guid id)
    {
        var entity = await _context.SiteDeployConfigs
            .Include(x => x.Origens)
            .FirstOrDefaultAsync(x => x.Id == id.ToString("D"));

        if (entity != null)
        {
            _context.SiteDeployConfigs.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
