using App.Application.Interfaces;
using App.Domain;
using App.Infrastructure.FileSystem;

namespace App.Infrastructure;

/// <summary>
/// Realiza deployment por cópia de arquivos usando staging temporário.
/// </summary>
public sealed class FileSystemDirectoryDeployer : IDirectoryDeployer
{
    #region Public Methods
    /// <summary>
    /// Copia todas as origens para staging e aplica no destino final.
    /// </summary>
    public void DeployFromOrigins(IReadOnlyList<DeploymentOrigin> origins, string? svn, string destinationRoot)
    {
        Console.WriteLine("Copiando arquivos para staging (memória/disco temporário)...");

        var stagingRoot = Path.Combine(Path.GetTempPath(), "App_deploy", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(stagingRoot);

        var destinationLeaf = new DirectoryInfo(Path.GetFullPath(destinationRoot.Trim())).Name;

        try
        {
            // Copia todas as origens para o staging
            foreach (var origin in origins)
            {
                StageOrigin(origin, stagingRoot, destinationLeaf);
            }

            Console.WriteLine("Aplicando staging no destino...");

            var normalizedDestination = PrepareDestination(destinationRoot);
            DirectoryHelper.CopyDirectoryContents(stagingRoot, normalizedDestination, overwrite: true);
        }
        finally
        {
            try
            {
                if (Directory.Exists(stagingRoot))
                {
                    DirectoryHelper.ClearDirectoryContents(stagingRoot);
                    Directory.Delete(stagingRoot, recursive: true);
                }
            }
            catch
            {
                // best-effort cleanup
            }
        }
    }

    /// <summary>
    /// Normaliza e valida um diretório de destino existente.
    /// </summary>
    public string NormalizeExistingDestination(string destinationRoot)
    {
        if (string.IsNullOrWhiteSpace(destinationRoot))
            throw new InvalidOperationException("O campo destino não foi informado.");

        var normalized = Path.GetFullPath(destinationRoot.Trim());

        if (!Directory.Exists(normalized))
            throw new InvalidOperationException($"O destino {normalized} não existe.");

        return normalized;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Prepara e limpa o diretório de destino antes da cópia.
    /// </summary>
    private static string PrepareDestination(string destinationRoot)
    {
        if (string.IsNullOrWhiteSpace(destinationRoot))
            throw new InvalidOperationException("Destino está vazio.");

        if (File.Exists(destinationRoot))
            throw new InvalidOperationException($"Destino aponta para um arquivo: {destinationRoot}");

        var normalized = Path.GetFullPath(destinationRoot.Trim());

        if (IsDangerousDestination(normalized))
            throw new InvalidOperationException($"Destino perigoso: {normalized}");

        if (!Directory.Exists(normalized))
            throw new InvalidOperationException($"Destino não existe: {normalized}");

        Console.WriteLine($"Limpando destino antes de copiar: {normalized}");
        DirectoryHelper.ClearDirectoryContents(normalized);

        return normalized;
    }

    /// <summary>
    /// Evita que o deploy seja executado em diretórios perigosos.
    /// </summary>
    private static bool IsDangerousDestination(string fullPath)
    {
        var root = Path.GetPathRoot(fullPath);

        if (!string.IsNullOrWhiteSpace(root))
        {
            var trimmed = fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var trimmedRoot = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (string.Equals(trimmed, trimmedRoot, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        var lastSegment = new DirectoryInfo(fullPath).Name;

        return string.IsNullOrWhiteSpace(lastSegment) || lastSegment.Length < 2;
    }

    /// <summary>
    /// Copia uma origem para o diretório de staging.
    /// </summary>
    private static void StageOrigin(DeploymentOrigin origin, string stagingRoot, string destinationLeaf)
    {
        var originPath = origin.Path;

        if (File.Exists(originPath))
        {
            var fileName = Path.GetFileName(originPath);
            var destFile = Path.Combine(stagingRoot, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
            File.Copy(originPath, destFile, overwrite: true);
            return;
        }

        if (Directory.Exists(originPath))
        {
            // Copia só o conteúdo interno da pasta
            if (origin.Conteudo)
            {
                DirectoryHelper.CopyDirectoryContents(originPath, stagingRoot, overwrite: true);
                return;
            }

            var dirName = new DirectoryInfo(originPath).Name;

            // fallback: se nome da origem = nome do destino, copia só conteúdo
            if (!string.IsNullOrWhiteSpace(destinationLeaf) &&
                string.Equals(dirName, destinationLeaf, StringComparison.OrdinalIgnoreCase))
            {
                DirectoryHelper.CopyDirectoryContents(originPath, stagingRoot, overwrite: true);
                return;
            }

            // comportamento padrão: copia a pasta inteira
            var destDir = Path.Combine(stagingRoot, dirName);
            Directory.CreateDirectory(destDir);

            DirectoryHelper.CopyDirectoryContents(originPath, destDir, overwrite: true);
            return;
        }

        throw new FileNotFoundException($"Origem não encontrada: {originPath}");
    }
    #endregion
}