using OneDriveAndroidSync.Infrastructure;
using Xunit;

namespace OneDriveAndroidSync.Infrastructure.Tests;

public sealed class SyncHistoryStoreTests
{
    [Fact]
    public async Task SessionAndAttempt_ArePersistedAndSessionCanBeRead()
    {
        var root = Directory.CreateTempSubdirectory("onedrive-sync-");
        try
        {
            var store = new SyncHistoryStore(Path.Combine(root.FullName, "history.db"));
            await store.InitializeAsync();
            var id = Guid.NewGuid();
            var session = new SyncSessionRecord(
                id,
                DateTime.UtcNow,
                null,
                root.FullName,
                "/sdcard/DCIM",
                "device-1",
                "Running");

            await store.SaveSessionAsync(session);
            await store.SaveAttemptAsync(new TransferAttemptRecord(
                Guid.NewGuid(), id, "photo.jpg", "/sdcard/DCIM/photo.jpg", "Succeeded", 10, null));

            var result = await store.FindSessionAsync(id);

            Assert.Equal(session, result);
        }
        finally
        {
            root.Delete(true);
        }
    }
}
