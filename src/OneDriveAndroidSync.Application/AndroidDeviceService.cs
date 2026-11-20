namespace OneDriveAndroidSync.Application;

public sealed class AndroidDeviceService(IAdbClient adbClient) : IAndroidDeviceService
{
    private IReadOnlyList<AdbDeviceInfo> devices = Array.Empty<AdbDeviceInfo>();

    public AdbDeviceInfo? SelectedDevice { get; private set; }

    public async Task<IReadOnlyList<AdbDeviceInfo>> RefreshAsync(
        CancellationToken cancellationToken = default)
    {
        devices = await adbClient.ListDevicesAsync(cancellationToken);

        if (SelectedDevice is not null &&
            devices.All(device => device.Id != SelectedDevice.Id || device.State != AdbDeviceState.Device))
        {
            SelectedDevice = null;
        }

        return devices;
    }

    public async Task SelectAsync(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            throw new ArgumentException("O identificador do dispositivo é obrigatório.", nameof(deviceId));
        }

        if (devices.Count == 0)
        {
            await RefreshAsync(cancellationToken);
        }

        var device = devices.SingleOrDefault(item => item.Id == deviceId);
        if (device is null)
        {
            throw new InvalidOperationException($"Dispositivo não encontrado: {deviceId}");
        }

        if (device.State != AdbDeviceState.Device)
        {
            throw new InvalidOperationException(
                $"O dispositivo {deviceId} não está autorizado para uso: {device.State}.");
        }

        SelectedDevice = device;
    }

    public Task<IReadOnlyList<RemoteFileInfo>> ListDestinationAsync(
        string remotePath,
        CancellationToken cancellationToken = default)
    {
        if (SelectedDevice is null)
        {
            throw new InvalidOperationException("É necessário selecionar um dispositivo autorizado.");
        }

        if (string.IsNullOrWhiteSpace(remotePath) || !remotePath.StartsWith('/'))
        {
            throw new ArgumentException(
                "O caminho remoto deve ser absoluto e começar por '/'.",
                nameof(remotePath));
        }

        return adbClient.ListDirectoryAsync(
            SelectedDevice.Id,
            remotePath,
            cancellationToken);
    }
}
