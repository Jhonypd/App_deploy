using System.Diagnostics;

namespace App.Infrastructure.Helpers;

public static class SvnHelper
{
    public static void RunUpdate(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("O caminho para atualização SVN não foi informado.", nameof(path));
        }

        Console.WriteLine($"Atualizando SVN: {path}");

        var psi = new ProcessStartInfo
        {
            FileName = "svn",
            Arguments = $"update \"{path}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Não foi possível iniciar o processo SVN.");

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Erro ao executar SVN update em '{path}'.\n\nSaída:\n{output}\n\nErro:\n{error}");
        }

        if (!string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine(output);
        }
    }
}