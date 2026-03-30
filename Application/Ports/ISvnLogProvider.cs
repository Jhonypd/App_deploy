using App.Domain;

namespace App.Application.Ports;

public interface ISvnLogProvider
{
	IReadOnlyList<SvnCommit> GetCommits(string workingCopyPathOrUrl, int limit);
}
