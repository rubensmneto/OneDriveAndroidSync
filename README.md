# OneDriveAndroidSync

Aplicação desktop Windows para transferir arquivos de uma pasta local sincronizada pelo OneDrive para um dispositivo Android via ADB.

## Estado

O projeto está na fase de criação da solução inicial. Consulte [`docs/STATUS.md`](docs/STATUS.md) para o andamento e os riscos conhecidos.

## Estrutura

- `src/OneDriveAndroidSync.App`: interface WPF e composição da aplicação.
- `src/OneDriveAndroidSync.Application`: casos de uso e contratos de aplicação.
- `src/OneDriveAndroidSync.Domain`: regras e modelos de domínio.
- `src/OneDriveAndroidSync.Infrastructure`: SQLite, filesystem, ADB e integrações externas.
- `src/OneDriveAndroidSync.Contracts`: DTOs e eventos compartilhados.
- `tests`: testes unitários e de integração.

Consulte `docs/CLAUDE_CODE_START_HERE.md` antes de continuar a implementação.
