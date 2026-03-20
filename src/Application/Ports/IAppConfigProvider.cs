using App_deploy.Domain;

namespace App_deploy.Application.Ports;

public interface IAppConfigProvider
{
	AppConfigRoot Load();
}
