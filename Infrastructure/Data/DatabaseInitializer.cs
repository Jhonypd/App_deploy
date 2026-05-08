using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.Data;

/// <summary>
/// Utilitário para inicializar o banco de dados manualmente.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Aplica todas as migrations pendentes ao banco de dados.
    /// </summary>
    public static async Task MigrateAsync(AppDbContext dbContext, ILogger? logger = null)
    {
        try
        {
            logger?.LogInformation("Iniciando migração do banco de dados...");

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (!pendingMigrations.Any())
            {
                logger?.LogInformation("Banco de dados já está atualizado. Nenhuma migração pendente.");
                return;
            }

            logger?.LogInformation($"Aplicando {pendingMigrations.Count()} migração(ões)...");
            await dbContext.Database.MigrateAsync();

            logger?.LogInformation("Migração concluída com sucesso!");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Erro ao migrar banco de dados");
            throw;
        }
    }

    /// <summary>
    /// Cria o banco de dados a partir do modelo atual (sem migrations).
    /// </summary>
    public static async Task CreateDatabaseAsync(AppDbContext dbContext, ILogger? logger = null)
    {
        try
        {
            logger?.LogInformation("Criando banco de dados...");
            await dbContext.Database.EnsureCreatedAsync();
            logger?.LogInformation("Banco de dados criado com sucesso!");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Erro ao criar banco de dados");
            throw;
        }
    }

    /// <summary>
    /// Remove o banco de dados (cuidado - destrói todos os dados).
    /// </summary>
    public static async Task DropDatabaseAsync(AppDbContext dbContext, ILogger? logger = null)
    {
        try
        {
            logger?.LogWarning("Removendo banco de dados...");
            await dbContext.Database.EnsureDeletedAsync();
            logger?.LogInformation("Banco de dados removido!");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Erro ao remover banco de dados");
            throw;
        }
    }
}
