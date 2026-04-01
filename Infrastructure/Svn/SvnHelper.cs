using System.Diagnostics;

namespace App.Infrastructure.Svn;

/// <summary>
/// Utilitário para execução de atualização SVN via TortoiseSVN.
/// </summary>
public static class SvnHelper
{
    #region Public Methods

    /// <summary>
    /// Executa atualização da working copy SVN no caminho informado.
    /// </summary>
    public static void RunUpdate(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("O caminho SVN não foi informado.", nameof(path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Diretório SVN não encontrado: {path}");

        var tortoiseProc = @"C:\Program Files\TortoiseSVN\bin\TortoiseProc.exe";

        if (!File.Exists(tortoiseProc))
            throw new FileNotFoundException($"TortoiseProc.exe não encontrado em: {tortoiseProc}");

        Console.WriteLine($"Atualizando SVN via TortoiseSVN: {path}");

        var psi = new ProcessStartInfo
        {
            FileName = tortoiseProc,
            Arguments = $"/command:update /path:\"{path}\" /closeonend:1",
            UseShellExecute = true,
            CreateNoWindow = false
        };

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Não foi possível iniciar o TortoiseProc.");

        // timeout de 2 minutos
        if (!process.WaitForExit(120000))
        {
            try
            {
                process.Kill();
            }
            catch
            {
                // ignora se já encerrou
            }

            throw new TimeoutException($"O SVN update demorou demais e foi encerrado: {path}");
        }

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Erro ao executar update SVN via TortoiseSVN em '{path}'. ExitCode: {process.ExitCode}");
        }

        Console.WriteLine("SVN atualizado com sucesso.");
    }

    #endregion
}