using OneDriveAndroidSync.Application;
using Xunit;

namespace OneDriveAndroidSync.Application.Tests;

public sealed class AndroidDeviceServiceTests
{
    [Fact]
    public async Task SelectAsync_AllowsAuthorizedDeviceAndListsDestination()
    {
        var fake = new FakeAdbClient(
            new AdbDeviceInfo("device-1", AdbDeviceState.Device, "Pixel"));
        var service = new AndroidDeviceService(fake);

        await service.SelectAsync("device-1");
        var files = await service.ListDestinationAsync("/sdcard/DCIM");

        Assert.Equal("device-1", service.SelectedDevice?.Id);
        Assert.Single(files);
        Assert.Equal("/sdcard/DCIM/photo.jpg", files[0].Path);
    }

    [Fact]
    public async Task SelectAsync_RejectsUnauthorizedDevice()
    {
        var service = new AndroidDeviceService(
            new FakeAdbClient(new AdbDeviceInfo("device-1", AdbDeviceState.Unauthorized)));

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SelectAsync("device-1"));

        Assert.Contains("não está autorizado", error.Message);
    }

    private sealed class FakeAdbClient(params AdbDeviceInfo[] devices) : IAdbClient
    {
        public Task<IReadOnlyList<AdbDeviceInfo>> ListDevicesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<AdbDeviceInfo>>(devices);

        public Task<IReadOnlyList<RemoteFileInfo>> ListDirectoryAsync(
            string deviceId,
            string remotePath,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<RemoteFileInfo>>(
                [new RemoteFileInfo($"{remotePath}/photo.jpg", 10, false)]);
    }
}
