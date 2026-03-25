using App.Application.Ports;
using App.Domain;
using App.Infrastructure.Helpers;

namespace App.Infrastructure;

public sealed class FileSystemDirectoryDeployer : IDirectoryDeployer
{
    public void DeployFromOrigins(IReadOnlyList<OrigensConfig> origins, string? svn, string destinationRoot)
    {
        Console.WriteLine("Copiando arquivos para staging (memória/disco temporário)...");

        var stagingRoot = Path.Combine(Path.GetTempPath(), "App_deploy", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(stagingRoot);

        var destinationLeaf = new DirectoryInfo(Path.GetFullPath(destinationRoot.Trim())).Name;

        try
        {
            // Roda o update da svn se veio o caminho a ser feito o update
            if (!string.IsNullOrWhiteSpace(svn))
            {
                SvnHelper.RunUpdate(svn);
            }

            foreach (var origin in origins)
            {
                StageOrigin(origin, stagingRoot, destinationLeaf);
            }

            Console.WriteLine("Aplicando staging no destino...");

            var normalizedDestination = PrepareDestination(destinationRoot);
            CopyDirectoryContents(stagingRoot, normalizedDestination, overwrite: true);
        }
        finally
        {
            try
            {
                if (Directory.Exists(stagingRoot))
                {
                    Directory.Delete(stagingRoot, recursive: true);
                }
            }
            catch
            {
                // best-effort cleanup
            }
        }
    }

    public string NormalizeExistingDestination(string destinationRoot)
    {
        if (string.IsNullOrWhiteSpace(destinationRoot))
            throw new InvalidOperationException("O campo destino não foi informado.");

        var normalized = Path.GetFullPath(destinationRoot.Trim());

        if (!Directory.Exists(normalized))
            throw new InvalidOperationException($"O destino {normalized} não existe.");

        return normalized;
    }

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
        ClearDirectoryContents(normalized);

        return normalized;
    }

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

    private static void ClearDirectoryContents(string directory)
    {
        foreach (var file in Directory.GetFiles(directory))
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (var dir in Directory.GetDirectories(directory))
        {
            Directory.Delete(dir, recursive: true);
        }
    }


    private static void StageOrigin(OrigensConfig origin, string stagingRoot, string destinationLeaf)
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
            if (origin.Conteudo)
            {
                CopyDirectoryContents(originPath, stagingRoot, overwrite: true);
                return;
            }

            var dirName = new DirectoryInfo(originPath).Name;

            if (!string.IsNullOrWhiteSpace(destinationLeaf) &&
                string.Equals(dirName, destinationLeaf, StringComparison.OrdinalIgnoreCase))
            {
                CopyDirectoryContents(originPath, stagingRoot, overwrite: true);
                return;
            }

            var destDir = Path.Combine(stagingRoot, dirName);
            Directory.CreateDirectory(destDir);

            CopyDirectoryContents(originPath, destDir, overwrite: true);
            return;
        }

        throw new FileNotFoundException($"Origem não encontrada: {originPath}");
    }

    private static void CopyDirectoryContents(string sourceDir, string destinationDir, bool overwrite)
    {
        foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceDir, dir);
            Directory.CreateDirectory(Path.Combine(destinationDir, relative));
        }

        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceDir, file);
            var destFile = Path.Combine(destinationDir, relative);

            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
            File.Copy(file, destFile, overwrite);
        }
    }
}