using System.Diagnostics;
using System.Globalization;
using OneDriveAndroidSync.Application;

namespace OneDriveAndroidSync.Infrastructure;

public sealed class AdbProcessClient(
    string adbExecutable = "adb",
    TimeSpan? commandTimeout = null) : IAdbClient, IAdbFileTransferClient
{
    private readonly TimeSpan timeout = commandTimeout ?? TimeSpan.FromMinutes(10);

    public async Task<IReadOnlyList<AdbDeviceInfo>> ListDevicesAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await RunAsync([], cancellationToken);
        var devices = new List<AdbDeviceInfo>();

        foreach (var line in result.StandardOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var fields = line.Trim().Split('\t', StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length < 2 || fields[0] == "List of devices attached")
            {
                continue;
            }

            var state = fields[1] switch
            {
                "device" => AdbDeviceState.Device,
                "unauthorized" => AdbDeviceState.Unauthorized,
                _ => AdbDeviceState.Offline
            };
            var model = fields.FirstOrDefault(field => field.StartsWith("model:", StringComparison.Ordinal))?
                .Split(':', 2)[1];
            devices.Add(new AdbDeviceInfo(fields[0], state, model));
        }

        return devices;
    }

    public async Task<IReadOnlyList<RemoteFileInfo>> ListDirectoryAsync(
        string deviceId,
        string remotePath,
        CancellationToken cancellationToken = default)
    {
        ValidateDeviceId(deviceId);
        ValidateRemotePath(remotePath);
        var result = await RunAsync(["-s", deviceId, "shell", "ls", "-l", remotePath], cancellationToken);
        var files = new List<RemoteFileInfo>();

        foreach (var line in result.StandardOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var fields = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length < 8 || fields[0] == "total")
            {
                continue;
            }

            var path = string.Join(' ', fields.Skip(7));
            if (!long.TryParse(fields[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out var size))
            {
                continue;
            }

            files.Add(new RemoteFileInfo(
                path.StartsWith('/') ? path : $"{remotePath.TrimEnd('/')}/{path}",
                size,
                fields[0][0] == 'd'));
        }

        return files;
    }

    public async Task PushFileAsync(
        string deviceId,
        string sourcePath,
        string remotePath,
        IProgress<TransferProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ValidateDeviceId(deviceId);
        ValidateRemotePath(remotePath);
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("O arquivo de origem não existe.", sourcePath);
        }

        var total = new FileInfo(sourcePath).Length;
        await RunAsync(["-s", deviceId, "push", sourcePath, remotePath], cancellationToken);
        progress?.Report(new TransferProgress(sourcePath, remotePath, total, total));
    }

    public async Task<long> GetRemoteFileSizeAsync(
        string deviceId,
        string remotePath,
        CancellationToken cancellationToken = default)
    {
        ValidateDeviceId(deviceId);
        ValidateRemotePath(remotePath);
        var result = await RunAsync(["-s", deviceId, "shell", "stat", "-c", "%s", remotePath], cancellationToken);
        if (!long.TryParse(result.StandardOutput.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var size))
        {
            throw new InvalidOperationException("O ADB retornou um tamanho remoto inválido.");
        }

        return size;
    }

    private async Task<ProcessResult> RunAsync(
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = adbExecutable,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        foreach (var argument in arguments)
        {
            process.StartInfo.ArgumentList.Add(argument);
        }

        if (!process.Start())
        {
            throw new InvalidOperationException("Não foi possível iniciar o executável ADB.");
        }

        using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCancellation.CancelAfter(timeout);
        var outputTask = process.StandardOutput.ReadToEndAsync(timeoutCancellation.Token);
        var errorTask = process.StandardError.ReadToEndAsync(timeoutCancellation.Token);
        await process.WaitForExitAsync(timeoutCancellation.Token);
        var output = await outputTask;
        var error = await errorTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Comando ADB falhou: {error.Trim()}");
        }

        return new ProcessResult(output, error);
    }

    private static void ValidateDeviceId(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId) || deviceId.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("O identificador ADB é inválido.", nameof(deviceId));
        }
    }

    private static void ValidateRemotePath(string remotePath)
    {
        if (string.IsNullOrWhiteSpace(remotePath) || !remotePath.StartsWith('/') || remotePath.Contains('\0'))
        {
            throw new ArgumentException("O caminho remoto deve ser absoluto e válido.", nameof(remotePath));
        }
    }

    private sealed record ProcessResult(string StandardOutput, string StandardError);
}
