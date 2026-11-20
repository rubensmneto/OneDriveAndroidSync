using OneDriveAndroidSync.Application;
using Xunit;

namespace OneDriveAndroidSync.Application.Tests;

public sealed class FileSourceScannerTests
{
    [Fact]
    public async Task ScanAsync_FindsFilesRecursivelyWithUtcFingerprint()
    {
        var root = Directory.CreateTempSubdirectory("onedrive-sync-");
        try
        {
            var nested = Directory.CreateDirectory(Path.Combine(root.FullName, "nested"));
            var filePath = Path.Combine(nested.FullName, "20240101_120000.jpg");
            await File.WriteAllTextAsync(filePath, "content");

            var results = new List<OneDriveAndroidSync.Domain.FileFingerprint>();
            await foreach (var fingerprint in new FileSourceScanner().ScanAsync(root.FullName))
            {
                results.Add(fingerprint);
            }

            var result = Assert.Single(results);
            Assert.Equal(Path.GetFullPath(filePath), result.FullPath);
            Assert.Equal(new FileInfo(filePath).Length, result.Size);
            Assert.Equal(DateTimeKind.Utc, result.LastWriteTimeUtc.Kind);
        }
        finally
        {
            root.Delete(true);
        }
    }

    [Fact]
    public async Task ScanAsync_StopsWhenCancelled()
    {
        var root = Directory.CreateTempSubdirectory("onedrive-sync-");
        try
        {
            await File.WriteAllTextAsync(Path.Combine(root.FullName, "file.txt"), "content");
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var _ in new FileSourceScanner().ScanAsync(root.FullName, cancellation.Token))
                {
                }
            });
        }
        finally
        {
            root.Delete(true);
        }
    }
}
