using System.Diagnostics;
using System.Xml.Linq;
using App.Application.Ports;
using App.Domain;

namespace App.Infrastructure;

public sealed class SvnCliLogProvider : ISvnLogProvider
{
	public IReadOnlyList<SvnCommit> GetCommits(string workingCopyPathOrUrl, int limit)
	{
		if (string.IsNullOrWhiteSpace(workingCopyPathOrUrl))
		{
			throw new ArgumentException("O caminho/URL do SVN não foi informado.", nameof(workingCopyPathOrUrl));
		}

		if (limit <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(limit), "O limit deve ser maior que zero.");
		}

		var cappedLimit = Math.Min(limit, 200);
		var svnExe = ResolveSvnExe();

		var psi = new ProcessStartInfo
		{
			FileName = svnExe,
			Arguments = $"log --xml -l {cappedLimit} --non-interactive \"{workingCopyPathOrUrl}\"",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true
		};

		if (Directory.Exists(workingCopyPathOrUrl))
		{
			psi.WorkingDirectory = workingCopyPathOrUrl;
		}

		Process? process;
		try
		{
			process = Process.Start(psi);
		}
		catch (Exception ex)
		{
throw new InvalidOperationException(
				"Não foi possível executar o comando 'svn'. Verifique se o Subversion CLI está instalado " +
				"(ex.: TortoiseSVN) e se 'svn.exe' está disponível no PATH, ou em 'C:\\Program Files\\TortoiseSVN\\bin'.",
				ex);
		}

		if (process is null)
		{
			throw new InvalidOperationException("Não foi possível iniciar o processo do SVN.");
		}

		var stdout = process.StandardOutput.ReadToEnd();
		var stderr = process.StandardError.ReadToEnd();

		// timeout de 60s
		if (!process.WaitForExit(60_000))
		{
			try
			{
				process.Kill(entireProcessTree: true);
			}
			catch
			{
				// best-effort
			}

			throw new TimeoutException($"Timeout ao executar 'svn log' para '{workingCopyPathOrUrl}'.");
		}

		if (process.ExitCode != 0)
		{
			throw new InvalidOperationException(
				$"Erro ao executar 'svn log' para '{workingCopyPathOrUrl}'. ExitCode: {process.ExitCode}. {stderr}".Trim());
		}

		if (string.IsNullOrWhiteSpace(stdout))
		{
			return Array.Empty<SvnCommit>();
		}

		return ParseXml(stdout);
	}

	private static IReadOnlyList<SvnCommit> ParseXml(string xml)
	{
		var doc = XDocument.Parse(xml);
		var entries = doc.Descendants("logentry");

		var result = new List<SvnCommit>();
		foreach (var entry in entries)
		{
			var revAttr = entry.Attribute("revision")?.Value;
			_ = long.TryParse(revAttr, out var revision);

			var author = entry.Element("author")?.Value ?? string.Empty;
			var dateRaw = entry.Element("date")?.Value;
			var date = DateTimeOffset.MinValue;
			if (!string.IsNullOrWhiteSpace(dateRaw))
			{
				DateTimeOffset.TryParse(dateRaw, out date);
			}

			var message = entry.Element("msg")?.Value ?? string.Empty;
			result.Add(new SvnCommit
			{
				Revision = revision,
				Author = author,
				Date = date,
				Message = message
			});
		}

		return result;
	}

	private static string ResolveSvnExe()
	{
		// Caminhos comuns do TortoiseSVN
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

		// fallback: usa o svn do PATH; se não existir, o erro será mais claro no Process.Start
		return "svn";
	}
}
