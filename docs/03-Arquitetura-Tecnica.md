# OneDriveAndroidSync — Arquitetura técnica

## 1. Stack sugerida

- .NET 8 ou versão LTS vigente no ambiente do projeto.
- WPF com MVVM.
- SQLite via Entity Framework Core ou biblioteca equivalente.
- ADB executado como processo externo encapsulado por uma abstração.
- `System.IO` para varredura, streams e datas locais.
- SHA-256 ou algoritmo configurável para hash seletivo.
- Testes unitários e de integração com xUnit.

## 2. Solução sugerida

```text
OneDriveAndroidSync.sln
src/
  OneDriveAndroidSync.App/             # WPF, views, view models, DI, composição
  OneDriveAndroidSync.Application/     # casos de uso e contratos
  OneDriveAndroidSync.Domain/           # entidades, estados e regras
  OneDriveAndroidSync.Infrastructure/  # SQLite, ADB, OneDrive local, filesystem
  OneDriveAndroidSync.Contracts/       # DTOs e eventos, se necessário
tests/
  OneDriveAndroidSync.Domain.Tests/
  OneDriveAndroidSync.Application.Tests/
  OneDriveAndroidSync.Infrastructure.Tests/
docs/
```

## 3. Componentes principais

### `IFileSourceScanner`
Varre uma pasta raiz recursivamente, retorna metadados locais e detecta disponibilidade do conteúdo.

### `IFileNameDateParser`
Interpreta exclusivamente o prefixo/padrão configurado `yyyyMMdd_HHmmss`. Deve retornar sucesso, ausência de padrão ou data inválida.

### `ISynchronizationPlanner`
Compara origem, índice local e inventário remoto para gerar um plano de sincronização. Não deve executar comandos nem alterar arquivos.

### `IHashService`
Calcula hash somente quando solicitado. Deve operar em streams e suportar cancelamento.

### `IAdbClient`
Encapsula detecção de dispositivos, listagem de diretórios, criação de pasta, upload, download de backup, consulta de metadados e validação de tamanho. Nenhum outro componente deverá montar comandos ADB diretamente.

### `IAndroidDeviceService`
Usa `IAdbClient` para fornecer inventário do dispositivo, seleção, estado da conexão e pastas disponíveis.

### `ITransferService`
Executa uma decisão aprovada do plano: backup remoto quando necessário, cópia, aplicação de metadados possível e validação.

### `IMetadataService`
Lê metadados verificáveis da origem e tenta aplicá-los no destino. Deve retornar sucessos, limitações e avisos, nunca ocultar falhas parciais.

### `IOneDriveAvailabilityService`
Abstrai detecção e materialização de arquivos somente online. A primeira implementação deve funcionar com a pasta local do cliente OneDrive sem dependência direta da Graph API.

### `ISyncRepository`
Persiste sessões, arquivos, decisões, tentativas, resultados, hashes, avisos e erros no SQLite.

## 4. Fluxo de casos de uso

`CreateSyncPreviewUseCase`: valida configuração, varre origem, aplica filtro de datas, materializa arquivos online, consulta índice e dispositivo, gera plano.

`ResolvePreviewUseCase`: persiste decisões do usuário para arquivos fora do padrão e conflitos.

`ExecuteSyncUseCase`: executa somente o plano aprovado, publica progresso, suporta cancelamento e persiste cada transição.

`ReviewHistoryUseCase`: consulta sessões e detalhes.

## 5. Modelo de domínio

Entidades recomendadas: `SyncSession`, `SourceFile`, `RemoteFile`, `TransferPlanItem`, `TransferAttempt`, `MetadataSnapshot`, `ConflictDecision`, `BackupArtifact`, `DeviceSnapshot`.

Value objects: `DateRange`, `SourcePath`, `RemotePath`, `FileFingerprint`, `MetadataComparison`, `DeviceId`.

## 6. Regras de segurança técnica

- Nunca concatenar entrada do usuário em comandos sem escaping rigoroso.
- Preferir argumentos separados de processo quando possível.
- Validar e normalizar caminhos locais e remotos.
- Não permitir destino remoto fora do armazenamento permitido.
- Não gravar credenciais, tokens ou dados sensíveis no SQLite.
- Não executar exclusões remotas como parte da sincronização.
- Usar arquivos temporários e rename final quando a operação permitir.
- Registrar comandos de forma sanitizada, sem conteúdo sensível.

## 7. Testabilidade

Toda integração externa deverá ter interface. Testes unitários deverão cobrir parser de datas, intervalo inclusivo, colisões, estados, plano, cancelamento e decisões. Testes de integração deverão usar um fake de ADB e uma pasta temporária, sem depender de um celular real.

## 8. Observações sobre metadados

Não assumir que data de criação é gravável no Android. O contrato deve distinguir conteúdo transferido, metadados tentados, metadados aplicados e limitações. A validação mínima da primeira versão será tamanho remoto; validação por hash pode ser adicionada posteriormente.
