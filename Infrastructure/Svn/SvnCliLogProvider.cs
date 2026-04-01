using System.Diagnostics;
using System.Xml.Linq;
using App.Application.Interfaces;
using App.Domain;
using Microsoft.Extensions.Configuration;

namespace App.Infrastructure;

/// <summary>
/// Implementação de leitura de logs SVN usando a CLI nativa (svn.exe).
/// </summary>
public sealed class SvnCliLogProvider : ISvnLogProvider
{
	#region Fields

	private readonly IConfiguration? _configuration;

	#endregion

	#region Constructors

	/// <summary>
	/// Inicializa o provider com configuração opcional para caminho do svn.exe.
	/// </summary>
	public SvnCliLogProvider(IConfiguration? configuration = null)
	{
		_configuration = configuration;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Obtém os commits mais recentes do caminho informado.
	/// </summary>
	public IReadOnlyList<SvnCommit> GetCommits(string workingCopyPathOrUrl, int limit)
	{
		return GetCommitsInternal(workingCopyPathOrUrl, limit, startRevision: null);
	}

	public IReadOnlyList<SvnCommit> GetCommits(string workingCopyPathOrUrl, long startRevision, int limit)
	{
		return GetCommitsInternal(workingCopyPathOrUrl, limit, startRevision);
	}

	#endregion

	#region Private Methods

	private IReadOnlyList<SvnCommit> GetCommitsInternal(string workingCopyPathOrUrl, int limit, long? startRevision)
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
		var revRange = (startRevision.HasValue && startRevision.Value > 0)
			? $"-r {startRevision.Value}:0 "
			: string.Empty;

		var psi = new ProcessStartInfo
		{
			FileName = svnExe,
			Arguments = $"log --xml {revRange}-l {cappedLimit} --non-interactive \"{workingCopyPathOrUrl}\"",
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
			throw new InvalidOperationException(BuildSvnCliMissingMessage(svnExe), ex);
		}

		if (process is null)
		{
			throw new InvalidOperationException("Não foi possível iniciar o processo do SVN.");
		}

		var stdout = process.StandardOutput.ReadToEnd();
		var stderr = process.StandardError.ReadToEnd();

		// timeout de 15min (logs grandes / rede / servidor lento)
		if (!process.WaitForExit(900_000))
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

	private static string BuildSvnCliMissingMessage(string attemptedExe)
	{
		return
			$"Não foi possível executar o comando 'svn' (tentado: '{attemptedExe}'). " +
			"Para buscar logs (svn log) é necessário o 'svn.exe' (Subversion CLI). " +
			"O executável 'SubWCRevCOM.exe' do TortoiseSVN é para versionamento/substituição de revisão e NÃO fornece 'svn log'. " +
			"Soluções: (1) instalar/ajustar o TortoiseSVN com os Command Line Client Tools (para ter svn.exe), " +
			"(2) instalar SlikSVN/VisualSVN e garantir 'svn.exe' no PATH, " +
			"ou (3) configurar 'Svn:ExePath' no appsettings.json apontando para o svn.exe.";
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

	private string ResolveSvnExe()
	{
		var configured = _configuration?["Svn:ExePath"];
		if (!string.IsNullOrWhiteSpace(configured) && File.Exists(configured))
		{
			return configured;
		}

		// Caminhos comuns do TortoiseSVN
		var candidates = new[]
		{
			@"C:\Program Files\TortoiseSVN\bin\svn.exe",
			@"C:\Program Files (x86)\TortoiseSVN\bin\svn.exe"
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

	#endregion
}
