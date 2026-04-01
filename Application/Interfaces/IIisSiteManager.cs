namespace App.Application.Interfaces;

/// <summary>
/// Contrato para gerenciamento de ciclo de vida de sites no IIS.
/// </summary>
public interface IIisSiteManager
{
	#region Methods

	/// <summary>
	/// Para o site e o pool associado quando estiverem ativos.
	/// </summary>
	bool StopSiteIfRunning(string siteName);

	/// <summary>
	/// Inicia o site e o pool associado.
	/// </summary>
	void StartSite(string siteName);

	#endregion
}
