# Status — OneDriveAndroidSync

## Atualização

2026-11-20 — Classificação por data e planejamento incremental adicionados.

## Concluído

- Estrutura inicial da solução e projetos separados.
- Projetos WPF, domínio, aplicação, infraestrutura e contratos.
- Projetos de testes iniciais.
- Nullable reference types, implicit usings, análise estática e warnings como erros configurados.
- `.gitignore` para artefatos .NET, banco, logs, backups e mídia do usuário.
- README inicial.
- Parser inicial do padrão `yyyyMMdd_HHmmss`.
- `DateRange` com intervalo inclusivo e validação do intervalo.
- Parser com cobertura de nomes ausentes e datas inválidas.
- Scanner recursivo com fingerprints de caminho, tamanho e data de alteração UTC.
- Índice SQLite inicial com criação de schema, upsert e consulta por caminho.
- Classificação de arquivos em elegíveis, fora do intervalo, sem data e data inválida.
- Planejador incremental que compara fingerprints e separa arquivos inalterados de novos/alterados.
- Projetos de testes de aplicação e infraestrutura adicionados à solução.
- Logger básico em arquivo.
- Janela WPF inicial sem integração com dados reais.

## Validado

- SDK .NET 10.0.404 disponível no ambiente.
- Restore, build e testes executados após a correção do domínio.

## Próximos passos

Implementar scanner com tratamento observável de inacessíveis, contratos/fake de ADB e inventário do destino antes da integração real.

## Riscos conhecidos

- A materialização de arquivos somente online do OneDrive ainda precisa de uma implementação específica para Windows.
- A correspondência entre arquivos locais e remotos ainda precisa de regras formais.
- A aplicação e a validação de metadados dependem das capacidades do Android e do ADB.
- A infraestrutura SQLite inclui apenas o esqueleto inicial; entidades, migrações e transações ainda não foram implementadas.
