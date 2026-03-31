namespace App.Application.Ports;

public interface ISvnWorkingCopyUpdater
{
    void UpdateToRevision(string workingCopyPath, long revision);
    void UpdateToHead(string workingCopyPath);
}
