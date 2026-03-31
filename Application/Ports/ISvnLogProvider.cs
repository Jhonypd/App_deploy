using App.Domain;

namespace App.Application.Ports;

public interface ISvnLogProvider
{
	IReadOnlyList<SvnCommit> GetCommits(string workingCopyPathOrUrl, int limit);
	IReadOnlyList<SvnCommit> GetCommits(string workingCopyPathOrUrl, long startRevision, int limit);
}
