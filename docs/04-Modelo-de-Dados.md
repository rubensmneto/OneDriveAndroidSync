# OneDriveAndroidSync — Modelo de dados inicial

## SyncSessions

- `Id` — identificador da sessão.
- `StartedAt`, `FinishedAt`.
- `DateRangeStart`, `DateRangeEnd`.
- `SourceRoot`.
- `RemoteDestination`.
- `SelectedDeviceId`.
- `Status`.
- contadores do resumo.

## SourceFiles

- `Id`.
- `FullPath`.
- `RelativePath`.
- `FileName`.
- `Size`.
- `LastWriteTimeUtc`.
- `ParsedNameDate`.
- `NameParseStatus`.
- `ContentHash` opcional.
- `MetadataFingerprint` opcional.
- `FirstSeenAt`, `LastScannedAt`.
- `LastSuccessfulSessionId`.

## RemoteFiles

- `Id`.
- `DeviceId`.
- `RemotePath`.
- `FileName`.
- `Size`.
- `LastWriteTime` quando disponível.
- `CreationTime` quando disponível.
- `ContentHash` opcional.
- `MetadataFingerprint` opcional.
- `LastObservedAt`.

## TransferPlanItems

- `Id`.
- `SessionId`.
- `SourceFileId`.
- `RemoteFileId` opcional.
- `PlannedRemotePath`.
- `Classification`.
- `Decision`.
- `DecisionReason`.
- `RequiresBackup`.
- `RequiresMetadataUpdate`.
- `Status`.

## TransferAttempts

- `Id`.
- `PlanItemId`.
- `StartedAt`, `FinishedAt`.
- `Status`.
- `BytesTransferred`.
- `RemoteSizeAfter`.
- `ValidationStatus`.
- `ErrorCode`.
- `ErrorMessage`.

## Backups

- `Id`.
- `PlanItemId`.
- `SessionId`.
- `LocalBackupPath`.
- `OriginalRemotePath`.
- `Size`.
- `CreatedAt`.
- `ValidationStatus`.

## MetadataOperations

- `Id`.
- `PlanItemId`.
- `MetadataName`.
- `SourceValue`.
- `AttemptedValue`.
- `Result`.
- `Message`.

## Configurations

- `Key`.
- `Value`.
- `UpdatedAt`.

Não armazenar fotos dentro do banco. O SQLite deve armazenar índice, decisões, hashes, estados e referências a caminhos.
