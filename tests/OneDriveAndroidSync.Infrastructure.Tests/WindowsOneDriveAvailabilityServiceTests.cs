using OneDriveAndroidSync.Infrastructure;
using Xunit;

namespace OneDriveAndroidSync.Infrastructure.Tests;

public sealed class WindowsOneDriveAvailabilityServiceTests
{
    [Fact]
    public async Task CheckAsync_ReportsMaterializedFile()
    {
        var root = Directory.CreateTempSubdirectory("onedrive-sync-");
        try
        {
            var path = Path.Combine(root.FullName, "photo.jpg");
            await File.WriteAllTextAsync(path, "content");

            var result = await new WindowsOneDriveAvailabilityService().CheckAsync(path);

            Assert.True(result.IsAvailable);
            Assert.True(result.IsMaterialized);
        }
        finally
        {
            root.Delete(true);
        }
    }

    [Fact]
    public async Task MaterializeAsync_ExplainsMissingFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "photo.jpg");

        var result = await new WindowsOneDriveAvailabilityService().MaterializeAsync(path);

        Assert.False(result.IsAvailable);
        Assert.False(result.IsMaterialized);
        Assert.Contains("somente online", result.Message);
    }
}
