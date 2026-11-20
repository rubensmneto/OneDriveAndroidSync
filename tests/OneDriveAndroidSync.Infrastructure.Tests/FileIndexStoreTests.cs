using OneDriveAndroidSync.Domain;
using OneDriveAndroidSync.Infrastructure;
using Xunit;

namespace OneDriveAndroidSync.Infrastructure.Tests;

public sealed class FileIndexStoreTests
{
    [Fact]
    public async Task UpsertAndFind_PersistsFingerprint()
    {
        var root = Directory.CreateTempSubdirectory("onedrive-sync-");
        try
        {
            var databasePath = Path.Combine(root.FullName, "index.db");
            var fingerprint = new FileFingerprint(
                Path.Combine(root.FullName, "photo.jpg"),
                123,
                new DateTime(2024, 1, 1, 12, 30, 0, DateTimeKind.Utc));
            var store = new FileIndexStore(databasePath);

            await store.InitializeAsync();
            await store.UpsertAsync(fingerprint);
            var result = await store.FindAsync(fingerprint.FullPath);

            Assert.Equal(fingerprint, result);
        }
        finally
        {
            root.Delete(true);
        }
    }
}
