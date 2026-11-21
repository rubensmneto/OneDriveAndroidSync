using OneDriveAndroidSync.Application;
using Xunit;

namespace OneDriveAndroidSync.Application.Tests;

public sealed class TransferServiceTests
{
    [Fact]
    public async Task TransferAsync_PushesAndValidatesRemoteSize()
    {
        var root = Directory.CreateTempSubdirectory("onedrive-sync-");
        try
        {
            var source = Path.Combine(root.FullName, "photo.jpg");
            await File.WriteAllTextAsync(source, "content");
            var device = new FakeDeviceService();
            var client = new FakeTransferClient(new FileInfo(source).Length);
            var service = new TransferService(client, device);

            var result = await service.TransferAsync(
                new TransferRequest(source, "/sdcard/DCIM/photo.jpg", new FileInfo(source).Length));

            Assert.True(result.Validated);
            Assert.Equal("device-1", client.DeviceId);
            Assert.Equal(source, client.SourcePath);
        }
        finally
        {
            root.Delete(true);
        }
    }

    [Fact]
    public async Task TransferAsync_RejectsChangedSourceBeforePush()
    {
        var root = Directory.CreateTempSubdirectory("onedrive-sync-");
        try
        {
            var source = Path.Combine(root.FullName, "photo.jpg");
            await File.WriteAllTextAsync(source, "changed");
            var service = new TransferService(
                new FakeTransferClient(10),
                new FakeDeviceService());

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.TransferAsync(
                new TransferRequest(source, "/sdcard/DCIM/photo.jpg", 10)));
        }
        finally
        {
            root.Delete(true);
        }
    }

    private sealed class FakeDeviceService : IAndroidDeviceService
    {
        public AdbDeviceInfo? SelectedDevice { get; } =
            new("device-1", AdbDeviceState.Device);

        public Task<IReadOnlyList<AdbDeviceInfo>> RefreshAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<AdbDeviceInfo>>(
                [SelectedDevice!]);

        public Task SelectAsync(string deviceId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<IReadOnlyList<RemoteFileInfo>> ListDestinationAsync(
            string remotePath,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<RemoteFileInfo>>([]);
    }

    private sealed class FakeTransferClient(long remoteSize) : IAdbFileTransferClient
    {
        public string? DeviceId { get; private set; }
        public string? SourcePath { get; private set; }

        public Task PushFileAsync(
            string deviceId,
            string sourcePath,
            string remotePath,
            IProgress<TransferProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            DeviceId = deviceId;
            SourcePath = sourcePath;
            return Task.CompletedTask;
        }

        public Task<long> GetRemoteFileSizeAsync(
            string deviceId,
            string remotePath,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(remoteSize);
    }
}
