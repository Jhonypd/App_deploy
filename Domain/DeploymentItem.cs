namespace App.Domain;

/// <summary>
/// Representa um item configurado para deployment.
/// </summary>
public sealed class DeploymentItem
{
	#region Properties
	/// <summary>
	/// Identificador único do deployment.
	/// </summary>
	public string Id { get; init; } = string.Empty;

	/// <summary>
	/// Nome do site no IIS.
	/// </summary>
	public string NomeSite { get; init; } = string.Empty;

	/// <summary>
	/// Caminho do working copy SVN.
	/// </summary>
	public string Svn { get; set; } = string.Empty;

	/// <summary>
	/// Lista de origens de arquivos a serem copiadas.
	/// </summary>
	public List<DeploymentOrigin> Origins { get; init; } = new();

	/// <summary>
	/// Caminho de destino final do deployment.
	/// </summary>
	public string Destino { get; init; } = string.Empty;
	#endregion
}

/// <summary>
/// Representa uma origem individual de arquivos para deployment.
/// </summary>
public sealed class DeploymentOrigin
{
	#region Properties
	/// <summary>
	/// Caminho de arquivo ou diretório de origem.
	/// </summary>
	public string Path { get; set; } = string.Empty;

	/// <summary>
	/// Indica se apenas o conteúdo da pasta deve ser copiado.
	/// </summary>
	public bool Conteudo { get; set; }
	#endregion

}