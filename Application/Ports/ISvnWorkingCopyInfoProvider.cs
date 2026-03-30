namespace App.Application.Ports;

public interface ISvnWorkingCopyInfoProvider
{
    long GetCurrentRevision(string workingCopyPath);
}
