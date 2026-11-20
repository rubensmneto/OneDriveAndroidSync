# Pacote de documentação — OneDriveAndroidSync

Este pacote contém os documentos iniciais para o desenvolvimento do OneDriveAndroidSync:

- `01-PRD-OneDriveAndroidSync.md`: visão do produto, objetivos, escopo e requisitos de negócio.
- `02-Especificacao-Funcional.md`: fluxos, estados, telas e comportamento esperado.
- `03-Arquitetura-Tecnica.md`: arquitetura, componentes, contratos e regras técnicas.
- `04-Modelo-de-Dados.md`: entidades e campos iniciais do SQLite.
- `05-Plano-de-Implementacao.md`: fases de desenvolvimento e Definition of Done.
- `06-Criterios-de-Aceite.md`: cenários que devem ser atendidos para considerar o produto funcional.
- `CLAUDE_CODE_START_HERE.md`: instruções para o Claude Code iniciar a implementação e o versionamento.

## Como usar

Copie os arquivos para a raiz do repositório `OneDriveAndroidSync` ou para uma pasta `docs/`, mantendo o `CLAUDE_CODE_START_HERE.md` facilmente acessível.

No Claude Code, solicite primeiro a leitura de `CLAUDE_CODE_START_HERE.md` e de todos os documentos relacionados. O agente deverá inspecionar o repositório antes de criar código.

## Observações técnicas importantes

O projeto deve tratar metadados como operações best-effort. A cópia do conteúdo e a preservação da data de criação do sistema de arquivos são capacidades diferentes; o Android pode não permitir a alteração de todos os atributos.

A integração com OneDrive será baseada inicialmente na pasta local sincronizada. A aplicação deverá abstrair a materialização de arquivos somente online para permitir uma implementação futura mais robusta.

A primeira versão não deverá verificar a conclusão do backup na nuvem, não deverá excluir arquivos do celular e não deverá executar sincronização bidirecional.
