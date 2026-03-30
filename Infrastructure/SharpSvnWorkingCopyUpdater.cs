using App.Application.Ports;
using SharpSvn;

namespace App.Infrastructure;

public sealed class SharpSvnWorkingCopyUpdater : ISvnWorkingCopyUpdater
{
    public void UpdateToRevision(string workingCopyPath, long revision)
    {
        if (string.IsNullOrWhiteSpace(workingCopyPath))
        {
            throw new ArgumentException("O caminho do working copy SVN não foi informado.", nameof(workingCopyPath));
        }

        if (!Directory.Exists(workingCopyPath))
        {
            throw new DirectoryNotFoundException($"Diretório SVN não encontrado: {workingCopyPath}");
        }

        if (revision <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision), "A revision deve ser maior que zero.");
        }

        using var client = new SvnClient();
        var args = new SvnUpdateArgs
        {
            Revision = new SvnRevision(revision)
        };

        try
        {
            var fullPath = Path.GetFullPath(workingCopyPath.Trim());
            client.Update(fullPath, args);
        }
        catch (SvnException ex)
        {
            throw new InvalidOperationException(
                $"Erro ao executar 'svn update -r {revision}' via SharpSvn em '{workingCopyPath}'. {ex.Message}",
                ex);
        }
    }
}
