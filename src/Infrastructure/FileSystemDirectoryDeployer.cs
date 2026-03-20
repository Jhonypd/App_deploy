using App_deploy.Application.Ports;

namespace App_deploy.Infrastructure;

public sealed class FileSystemDirectoryDeployer : IDirectoryDeployer
{
	public void DeployFromOrigins(IReadOnlyList<string> originPaths, string destinationRoot)
	{
		Console.WriteLine("Copiando arquivos para staging (memória/disco temporário)...");
		var stagingRoot = Path.Combine(Path.GetTempPath(), "App_deploy", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(stagingRoot);
		var destinationLeaf = new DirectoryInfo(Path.GetFullPath(destinationRoot.Trim())).Name;

		try
		{
			foreach (var origin in originPaths)
			{
				StageOrigin(origin, stagingRoot, destinationLeaf);
			}

			Console.WriteLine("Aplicando staging no destino...");
			var normalizedDestination = PrepareDestination(destinationRoot);
			CopyDirectoryContents(stagingRoot, normalizedDestination, overwrite: true);
		}
		finally
		{
			try
			{
				if (Directory.Exists(stagingRoot))
				{
					Directory.Delete(stagingRoot, recursive: true);
				}
			}
			catch
			{
				// best-effort cleanup
			}
		}
	}

	public string NormalizeExistingDestination(string destinationRoot)
	{
		if (string.IsNullOrWhiteSpace(destinationRoot))
		{
			throw new InvalidOperationException("O campo 'destino' é obrigatório.");
		}

		var normalized = Path.GetFullPath(destinationRoot.Trim());
		if (!Directory.Exists(normalized))
		{
			throw new InvalidOperationException($"O destino não existe: {normalized}");
		}

		return normalized;
	}

	private static string PrepareDestination(string destinationRoot)
	{
		if (string.IsNullOrWhiteSpace(destinationRoot))
		{
			throw new InvalidOperationException("Destino vazio.");
		}

		if (File.Exists(destinationRoot))
		{
			throw new InvalidOperationException($"Destino aponta para um arquivo, esperado pasta: {destinationRoot}");
		}

		var normalized = Path.GetFullPath(destinationRoot.Trim());
		if (IsDangerousDestination(normalized))
		{
			throw new InvalidOperationException($"Destino perigoso para limpeza (recusado): {normalized}");
		}

		if (!Directory.Exists(normalized))
		{
			throw new InvalidOperationException($"Destino não existe: {normalized}");
		}

		Console.WriteLine($"Limpando destino antes de copiar: {normalized}");
		ClearDirectoryContents(normalized);
		return normalized;
	}

	private static bool IsDangerousDestination(string fullPath)
	{
		var root = Path.GetPathRoot(fullPath);
		if (!string.IsNullOrWhiteSpace(root))
		{
			var trimmed = fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			var trimmedRoot = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			if (string.Equals(trimmed, trimmedRoot, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		var lastSegment = new DirectoryInfo(fullPath).Name;
		if (string.IsNullOrWhiteSpace(lastSegment) || lastSegment.Length < 2)
		{
			return true;
		}

		return false;
	}

	private static void ClearDirectoryContents(string directory)
	{
		foreach (var file in Directory.GetFiles(directory))
		{
			File.SetAttributes(file, FileAttributes.Normal);
			File.Delete(file);
		}

		foreach (var dir in Directory.GetDirectories(directory))
		{
			Directory.Delete(dir, recursive: true);
		}
	}

	private static void StageOrigin(string originPath, string stagingRoot, string destinationLeaf)
	{
		if (File.Exists(originPath))
		{
			var fileName = Path.GetFileName(originPath);
			var destFile = Path.Combine(stagingRoot, fileName);
			Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
			File.Copy(originPath, destFile, overwrite: true);
			return;
		}

		if (Directory.Exists(originPath))
		{
			var dirName = new DirectoryInfo(originPath).Name;
			if (!string.IsNullOrWhiteSpace(destinationLeaf) &&
				string.Equals(dirName, destinationLeaf, StringComparison.OrdinalIgnoreCase))
			{
				CopyDirectoryContents(originPath, stagingRoot, overwrite: true);
				return;
			}

			var destDir = Path.Combine(stagingRoot, dirName);
			Directory.CreateDirectory(destDir);
			CopyDirectoryContents(originPath, destDir, overwrite: true);
			return;
		}

		throw new FileNotFoundException($"Origem não encontrada: {originPath}");
	}

	private static void CopyDirectoryContents(string sourceDir, string destinationDir, bool overwrite)
	{
		foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
		{
			var relative = Path.GetRelativePath(sourceDir, dir);
			Directory.CreateDirectory(Path.Combine(destinationDir, relative));
		}

		foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
		{
			var relative = Path.GetRelativePath(sourceDir, file);
			var destFile = Path.Combine(destinationDir, relative);
			Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
			File.Copy(file, destFile, overwrite);
		}
	}
}
