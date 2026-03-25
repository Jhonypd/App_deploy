namespace App.Api;

public sealed class ApiOptions
{
	public string? BaseUrl { get; init; }
	public string? ListenUrl { get; init; }
	public string? ApiKey { get; init; }
}
