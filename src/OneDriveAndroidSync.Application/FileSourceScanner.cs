using OneDriveAndroidSync.Domain;

namespace OneDriveAndroidSync.Application;

public sealed class FileSourceScanner : IFileSourceScanner
{
    public async IAsyncEnumerable<FileFingerprint> ScanAsync(
        string root,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            throw new ArgumentException("A pasta de origem é obrigatória.", nameof(root));
        }

        var fullRoot = Path.GetFullPath(root);
        if (!Directory.Exists(fullRoot))
        {
            throw new DirectoryNotFoundException($"A pasta de origem não existe: {fullRoot}");
        }

        foreach (var path in Directory.EnumerateFiles(fullRoot, "*", new EnumerationOptions
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
            ReturnSpecialDirectories = false
        }))
        {
            cancellationToken.ThrowIfCancellationRequested();

            FileInfo fileInfo;
            try
            {
                fileInfo = new FileInfo(path);
                if (!fileInfo.Exists)
                {
                    continue;
                }
            }
            catch (IOException)
            {
                continue;
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }

            yield return new FileFingerprint(
                fileInfo.FullName,
                fileInfo.Length,
                fileInfo.LastWriteTimeUtc);
            await Task.Yield();
        }
    }
}
