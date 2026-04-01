using System.IO;

namespace App.Infrastructure.FileSystem;

/// <summary>
/// Utilitarios de manipulacao de diretorios para rotinas de deployment.
/// </summary>
public static class DirectoryHelper
{
    #region Fields

    private static readonly HashSet<string> IgnoredDirectories = new(StringComparer.OrdinalIgnoreCase)
    {
        ".svn",
        ".git",
        ".vs"
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Remove todo o conteudo de um diretorio, incluindo subpastas.
    /// </summary>
    /// <param name="directory">Diretorio cujo conteudo deve ser apagado.</param>
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

    /// <summary>
    /// Copia recursivamente o conteudo de uma origem para um destino.
    /// </summary>
    /// <param name="sourceDir">Diretorio de origem.</param>
    /// <param name="destinationDir">Diretorio de destino.</param>
    /// <param name="overwrite">Indica se arquivos existentes devem ser sobrescritos.</param>
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

    /// <summary>
    /// Indica se o caminho informado esta dentro de uma pasta ignorada.
    /// </summary>
    /// <param name="path">Caminho a validar.</param>
    /// <returns><c>true</c> quando o caminho contem uma pasta ignorada; caso contrario, <c>false</c>.</returns>
    public static bool IsInsideIgnoredDirectory(string path)
    {
        var parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        return parts.Any(part => IgnoredDirectories.Contains(part));
    }

    /// <summary>
    /// Remove atributos de somente leitura de arquivos e pastas.
    /// </summary>
    /// <param name="directory">Diretorio alvo.</param>
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

    #endregion
}