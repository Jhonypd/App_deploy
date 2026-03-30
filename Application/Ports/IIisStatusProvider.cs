using App.Application;

namespace App.Application.Ports;

public interface IIisStatusProvider
{
	IReadOnlyList<SiteStatus> GetStatus();
}
