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

            // Para marcar o "commit atual" na lista de log do PATH, precisamos de uma revisão que exista no
            // histórico desse PATH. Em SVN, um working copy pode estar em uma revisão mais alta (ex.: 448), mas
            // o último commit que alterou esse PATH pode ser menor (ex.: 446). O svn log do PATH começa em 446.
            // Por isso, priorizamos LastChangeRevision.
            if (client.GetInfo(fullPath, out var info) && info is not null)
            {
                var lastChange = (long)info.LastChangeRevision;
                if (lastChange > 0)
                {
                    return lastChange;
                }

                var infoRevision = (long)info.Revision;
                if (infoRevision > 0)
                {
                    return infoRevision;
                }
            }

            // Fallback: tenta via status (WC base revision), útil se o Info falhar.
            var statusArgs = new SvnStatusArgs
            {
                Depth = SvnDepth.Empty,
                RetrieveAllEntries = true
            };

            if (client.GetStatus(fullPath, statusArgs, out var statuses) && statuses is not null)
            {
                var root = statuses.FirstOrDefault();
                if (root is not null)
                {
                    var statusRevision = (long)root.Revision;
                    if (statusRevision > 0)
                    {
                        return statusRevision;
                    }
                }
            }

            return 0;
        }
        catch (SvnException ex)
        {
            throw new InvalidOperationException(
                $"Erro ao obter revisão atual via SharpSvn em '{workingCopyPath}'. {ex.Message}",
                ex);
        }
    }
}
