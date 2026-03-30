using App.Application.Ports;
using SharpSvn;

namespace App.Infrastructure;

public sealed class SharpSvnWorkingCopyInfoProvider : ISvnWorkingCopyInfoProvider
{
    public long GetCurrentRevision(string workingCopyPath)
    {
        if (string.IsNullOrWhiteSpace(workingCopyPath))
        {
            throw new ArgumentException("O caminho do working copy SVN não foi informado.", nameof(workingCopyPath));
        }

        if (!Directory.Exists(workingCopyPath))
        {
            throw new DirectoryNotFoundException($"Diretório SVN não encontrado: {workingCopyPath}");
        }

        using var client = new SvnClient();
        try
        {
            var fullPath = Path.GetFullPath(workingCopyPath.Trim());
            if (!client.GetInfo(fullPath, out var info) || info is null)
            {
                throw new InvalidOperationException($"Não foi possível obter informações SVN do path: {workingCopyPath}");
            }

            return info.Revision;
        }
        catch (SvnException ex)
        {
            throw new InvalidOperationException(
                $"Erro ao obter revisão atual via SharpSvn em '{workingCopyPath}'. {ex.Message}",
                ex);
        }
    }
}
