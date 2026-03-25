using App.Domain;

namespace App.Application.Ports;

public interface IAppConfigProvider
{
	AppConfigRoot Load();
}
