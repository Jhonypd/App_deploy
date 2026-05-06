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

	/// <summary>
	/// Cria ou atualiza as configurações de um site no IIS.
	/// </summary>
	void CriarOuAtualizarSite(string nomeSite, string caminhoFisico, int porta = 80);

	/// <summary>
	/// Remove um site do IIS, se existir.
	/// </summary>
	void RemoverSite(string nomeSite);

	/// <summary>
	/// Cria o diretório de destino se não existir.
	/// </summary>
	void CriarDiretorioSeNaoExistir(string caminho);

	#endregion
}
