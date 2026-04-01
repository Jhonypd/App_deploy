using System.Diagnostics;
using App.Application.Interfaces;

namespace App.Infrastructure.Svn;

/// <summary>
/// Implementação de atualização de working copy SVN usando CLI (svn.exe).
/// </summary>
public sealed class SvnCliWorkingCopyUpdater : ISvnWorkingCopyUpdater
{
    #region Constants

    private const int DefaultTimeoutMs = 900_000; // 15 minutos

    #endregion

    #region Public Methods

    /// <summary>
    /// Atualiza a working copy para uma revisão específica.
    /// </summary>

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

        var svnExe = ResolveSvnExe();

        var psi = new ProcessStartInfo
        {
            FileName = svnExe,
            Arguments = $"update -r {revision} --non-interactive --accept postpone \"{workingCopyPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = workingCopyPath
        };

        Process? process;
        try
        {
            process = Process.Start(psi);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Não foi possível executar o comando 'svn update'. Verifique se o Subversion CLI está instalado " +
                "(ex.: TortoiseSVN) e se 'svn.exe' está disponível no PATH, ou em 'C:\\Program Files\\TortoiseSVN\\bin'.",
                ex);
        }

        if (process is null)
        {
            throw new InvalidOperationException("Não foi possível iniciar o processo do SVN.");
        }

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();

        if (!process.WaitForExit(DefaultTimeoutMs))
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
                // best-effort
            }
            throw new TimeoutException($"Timeout ao executar 'svn update -r {revision}' em '{workingCopyPath}'.");
        }

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Erro ao executar 'svn update -r {revision}' em '{workingCopyPath}'. ExitCode: {process.ExitCode}. {stderr}\n{stdout}".Trim());
        }
    }

    /// <summary>
    /// Atualiza a working copy para a revisão HEAD.
    /// </summary>
    public void UpdateToHead(string workingCopyPath)
    {
        if (string.IsNullOrWhiteSpace(workingCopyPath))
        {
            throw new ArgumentException("O caminho do working copy SVN não foi informado.", nameof(workingCopyPath));
        }

        if (!Directory.Exists(workingCopyPath))
        {
            throw new DirectoryNotFoundException($"Diretório SVN não encontrado: {workingCopyPath}");
        }

        var svnExe = ResolveSvnExe();

        var psi = new ProcessStartInfo
        {
            FileName = svnExe,
            Arguments = $"update --non-interactive --accept postpone \"{workingCopyPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = workingCopyPath
        };

        Process? process;
        try
        {
            process = Process.Start(psi);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Não foi possível executar o comando 'svn update'. Verifique se o Subversion CLI está instalado " +
                "(ex.: TortoiseSVN) e se 'svn.exe' está disponível no PATH, ou em 'C:\\Program Files\\TortoiseSVN\\bin'.",
                ex);
        }

        if (process is null)
        {
            throw new InvalidOperationException("Não foi possível iniciar o processo do SVN.");
        }

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();

        if (!process.WaitForExit(DefaultTimeoutMs))
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
                // best-effort
            }
            throw new TimeoutException($"Timeout ao executar 'svn update' (HEAD) em '{workingCopyPath}'.");
        }

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Erro ao executar 'svn update' (HEAD) em '{workingCopyPath}'. ExitCode: {process.ExitCode}. {stderr}\n{stdout}".Trim());
        }
    }

    #endregion

    #region Private Methods

    private static string ResolveSvnExe()
    {
        var candidates = new[]
        {
            @"C:\\Program Files\\TortoiseSVN\\bin\\svn.exe",
            @"C:\\Program Files (x86)\\TortoiseSVN\\bin\\svn.exe"
        };

        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return "svn";
    }

    #endregion
}
