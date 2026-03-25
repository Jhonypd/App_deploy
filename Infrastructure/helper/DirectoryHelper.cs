using System.IO;

namespace App.Infrastructure.Helpers;

public static class DirectoryHelper
{
    private static readonly HashSet<string> IgnoredDirectories = new(StringComparer.OrdinalIgnoreCase)
    {
        ".svn",
        ".git",
        ".vs"
    };

    public static void ClearDirectoryContents(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        // Remove readonly/atributos de tudo antes de deletar
        RemoveReadOnlyFromDirectory(new DirectoryInfo(directory));

        // Deleta arquivos da raiz
        foreach (var file in Directory.GetFiles(directory))
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        // Deleta subpastas
        foreach (var dir in Directory.GetDirectories(directory))
        {
            var dirInfo = new DirectoryInfo(dir);
            RemoveReadOnlyFromDirectory(dirInfo);
            dirInfo.Delete(true);
        }
    }

    public static void CopyDirectoryContents(string sourceDir, string destinationDir, bool overwrite)
    {
        foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            if (IsInsideIgnoredDirectory(dir))
                continue;

            var relative = Path.GetRelativePath(sourceDir, dir);
            Directory.CreateDirectory(Path.Combine(destinationDir, relative));
        }

        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            if (IsInsideIgnoredDirectory(file))
                continue;

            var relative = Path.GetRelativePath(sourceDir, file);
            var destFile = Path.Combine(destinationDir, relative);

            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
            File.Copy(file, destFile, overwrite);
        }
    }

    public static bool IsInsideIgnoredDirectory(string path)
    {
        var parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        return parts.Any(part => IgnoredDirectories.Contains(part));
    }

    public static void RemoveReadOnlyFromDirectory(DirectoryInfo directory)
    {
        if (!directory.Exists)
            return;

        foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
        {
            file.Attributes = FileAttributes.Normal;
        }

        foreach (var dir in directory.GetDirectories("*", SearchOption.AllDirectories))
        {
            dir.Attributes = FileAttributes.Normal;
        }

        directory.Attributes = FileAttributes.Normal;
    }
}