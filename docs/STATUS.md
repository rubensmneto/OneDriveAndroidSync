# Status — OneDriveAndroidSync

## Atualização

2026-11-19 — Solução inicial criada conforme a arquitetura documentada.

## Concluído

- Estrutura inicial da solução e projetos separados.
- Projetos WPF, domínio, aplicação, infraestrutura e contratos.
- Projetos de testes iniciais.
- Nullable reference types, implicit usings, análise estática e warnings como erros configurados.
- `.gitignore` para artefatos .NET, banco, logs, backups e mídia do usuário.
- README inicial.
- Parser inicial do padrão `yyyyMMdd_HHmmss`.
- `DateRange` com intervalo inclusivo.
- Esqueleto de persistência SQLite.
- Logger básico em arquivo.
- Janela WPF inicial sem integração com dados reais.

## Não validado

O SDK .NET não está disponível neste ambiente, portanto `dotnet restore`, `dotnet build` e `dotnet test` ainda não puderam ser executados.

## Próximos passos

Implementar testes adicionais do parser, scanner recursivo, índice de arquivos e contratos/fake de ADB antes da integração real.

## Riscos conhecidos

- A materialização de arquivos somente online do OneDrive ainda precisa de uma implementação específica para Windows.
- A correspondência entre arquivos locais e remotos ainda precisa de regras formais.
- A aplicação e a validação de metadados dependem das capacidades do Android e do ADB.
- A infraestrutura SQLite inclui apenas o esqueleto inicial; entidades, migrações e transações ainda não foram implementadas.
