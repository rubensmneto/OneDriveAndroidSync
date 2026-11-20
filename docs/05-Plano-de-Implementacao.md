# OneDriveAndroidSync — Plano de implementação

## Fase 0 — Preparação

Criar a solução WPF, projetos de domínio, aplicação, infraestrutura e testes. Configurar analyzers, nullable reference types, tratamento de warnings, logging, `.editorconfig`, `.gitignore` e CI básico.

## Fase 1 — Domínio e parser

Implementar estados, value objects, `DateRange`, parser de nomes, regras de data inclusiva e classificação de arquivos. Criar testes abrangentes para nomes válidos, inválidos, datas impossíveis, extensões variadas, limites do intervalo e horário local.

## Fase 2 — Scanner e índice

Implementar scanner recursivo, fingerprint por caminho/tamanho/data, armazenamento SQLite e primeira varredura completa. Testar arquivos novos, alterados, removidos da origem e arquivos sem acesso.

## Fase 3 — ADB

Implementar wrapper de processo ADB, detecção de dispositivos, estados unauthorized/offline/device, seleção, listagem de diretórios e criação de diretórios. Criar fake de ADB e testes sem aparelho real.

## Fase 4 — OneDrive local

Implementar detecção de placeholders/arquivos somente online, materialização e timeout. Isolar a estratégia para permitir trocar a implementação no futuro.

## Fase 5 — Planejamento e prévia

Implementar comparação incremental, inventário remoto, colisões, conflitos de conteúdo/metadados, caminhos derivados da origem e tela de prévia com resumo e lista.

## Fase 6 — Transferência

Implementar backup remoto local, cópia, validação por tamanho, cancelamento, progresso, persistência por item, continuação após erro e interrupção segura na perda do dispositivo.

## Fase 7 — Metadados

Implementar leitura de metadados e tentativas de aplicação. Registrar resultado por metadado. Não bloquear cópia quando o Android não aceitar uma data física; mostrar alerta.

## Fase 8 — Interface

Implementar telas de configuração, seleção de intervalo, seleção de dispositivo/destino, prévia, progresso, resumo e histórico.

## Fase 9 — Qualidade

Adicionar testes de integração com filesystem temporário e fake ADB, testes de cancelamento, testes de reexecução, testes de colisões e testes de caminhos maliciosos. Executar build, testes e análise estática.

## Fase 10 — Empacotamento

Criar publicação para Windows, documentação de instalação do ADB, instruções de autorização USB, configuração inicial e troubleshooting. Não distribuir fotos, banco local, logs ou backups no repositório.

## Definition of Done

Uma etapa só estará concluída quando houver implementação, testes relevantes, documentação atualizada, build sem warnings novos e commit pequeno com mensagem clara.
