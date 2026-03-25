// using App.Domain;

// namespace App.Application.Ports;

// public interface IDirectoryDeployer
// {
//     void DeployFromOrigins(IReadOnlyList<OrigensConfig> origins, string? svn, string destinationRoot);

//     string NormalizeExistingDestination(string destinationRoot);
// }

using App.Domain;

namespace App.Application.Ports;

public interface IDirectoryDeployer
{
    void DeployFromOrigins(IReadOnlyList<OrigensConfig> origins, string? svn, string destinationRoot);
    string NormalizeExistingDestination(string destinationRoot);
}