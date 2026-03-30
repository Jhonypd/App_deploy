namespace App.Domain;

public sealed class DeploymentItem
{
	public string Id { get; init; } = string.Empty;

	public string NomeSite { get; init; } = string.Empty;

	public string Svn { get; set; } = string.Empty;

    public List<OrigensConfig> Origens { get; init; } = new();

	public string Destino { get; init; } = string.Empty;
}

public sealed class OrigensConfig
{
	public string Path { get; set; } = string.Empty;
	public bool Conteudo { get; set; }

    //public bool AtualizarSvn { get; set; }
}