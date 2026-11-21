namespace OneDriveAndroidSync.Application;

public sealed class TransferService(
    IAdbFileTransferClient transferClient,
    IAndroidDeviceService deviceService) : ITransferService
{
    public async Task<TransferResult> TransferAsync(
        TransferRequest request,
        IProgress<TransferProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SourcePath) || !File.Exists(request.SourcePath))
        {
            throw new FileNotFoundException("O arquivo de origem não existe.", request.SourcePath);
        }

        if (string.IsNullOrWhiteSpace(request.RemotePath) || !request.RemotePath.StartsWith('/'))
        {
            throw new ArgumentException(
                "O caminho remoto deve ser absoluto e começar por '/'.",
                nameof(request));
        }

        if (request.ExpectedSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "O tamanho esperado não pode ser negativo.");
        }

        var device = deviceService.SelectedDevice
            ?? throw new InvalidOperationException("É necessário selecionar um dispositivo autorizado.");

        var sourceSize = new FileInfo(request.SourcePath).Length;
        if (sourceSize != request.ExpectedSize)
        {
            throw new InvalidOperationException(
                $"O arquivo mudou antes da transferência: esperado {request.ExpectedSize}, encontrado {sourceSize}.");
        }

        await transferClient.PushFileAsync(
            device.Id,
            request.SourcePath,
            request.RemotePath,
            progress,
            cancellationToken);

        var remoteSize = await transferClient.GetRemoteFileSizeAsync(
            device.Id,
            request.RemotePath,
            cancellationToken);

        return new TransferResult(
            request.SourcePath,
            request.RemotePath,
            sourceSize,
            remoteSize == sourceSize,
            remoteSize == sourceSize ? null : $"Tamanho remoto {remoteSize} difere do local {sourceSize}.");
    }
}
