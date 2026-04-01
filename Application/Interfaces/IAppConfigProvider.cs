using App.Domain;

namespace App.Application.Interfaces;

/// <summary>
/// Contrato para leitura da configuração raiz da aplicação.
/// </summary>
public interface IAppConfigProvider
{
	#region Methods

	/// <summary>
	/// Carrega a configuração de deployment.
	/// </summary>
	AppConfigRoot Load();

	#endregion
}
