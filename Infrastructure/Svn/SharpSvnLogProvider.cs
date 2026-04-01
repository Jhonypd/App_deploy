using App.Application.Interfaces;
using App.Domain;
using SharpSvn;

namespace App.Infrastructure.Svn;

/// <summary>
/// Implementação de leitura de logs SVN usando SharpSvn.
/// </summary>
public sealed class SharpSvnLogProvider : ISvnLogProvider
{
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

	private static IReadOnlyList<SvnCommit> GetCommitsInternal(string workingCopyPathOrUrl, int limit, long? startRevision)
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

		using var client = new SvnClient();
		var args = new SvnLogArgs
		{
			Limit = cappedLimit,
			RetrieveAllProperties = false,
			RetrieveChangedPaths = false
		};

		if (startRevision.HasValue && startRevision.Value > 0)
		{
			args.Start = new SvnRevision(startRevision.Value);
			args.End = SvnRevision.Zero;
		}

		try
		{
			var result = new List<SvnCommit>();

			var trimmed = workingCopyPathOrUrl.Trim();
			if (Directory.Exists(trimmed) || File.Exists(trimmed))
			{
				var fullPath = Path.GetFullPath(trimmed);
				client.GetLog(fullPath, args, out var logItems);
				Append(result, logItems);
				return result;
			}

			if (Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
			{
				client.GetLog(uri, args, out var logItems);
				Append(result, logItems);
				return result;
			}

			// fallback: tenta como string (pode ser wc path relativo ou URL não-normalizada)
			client.GetLog(trimmed, args, out var fallbackItems);
			Append(result, fallbackItems);
			return result;
		}
		catch (SvnException ex)
		{
			throw new InvalidOperationException(
				$"Erro ao buscar logs SVN via SharpSvn para '{workingCopyPathOrUrl}'. {ex.Message}",
				ex);
		}
	}

	private static void Append(List<SvnCommit> result, ICollection<SvnLogEventArgs> logItems)
	{
		foreach (var item in logItems)
		{
			result.Add(new SvnCommit
			{
				Revision = item.Revision,
				Author = item.Author ?? string.Empty,
				Date = new DateTimeOffset(item.Time),
				Message = item.LogMessage ?? string.Empty
			});
		}
	}

	#endregion
}
