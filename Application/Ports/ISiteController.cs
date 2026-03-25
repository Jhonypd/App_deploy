namespace App.Application.Ports;

public interface ISiteController
{
	bool StopSiteIfRunning(string siteName);
	void StartSite(string siteName);
}
