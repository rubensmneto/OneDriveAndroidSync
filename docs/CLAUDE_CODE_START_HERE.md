# Instruções iniciais para o Claude Code

## Contexto

Você vai implementar o projeto OneDriveAndroidSync, cujo repositório é:

`https://github.com/rubensmneto/OneDriveAndroidSync`

Leia primeiro todos os documentos desta pasta, especialmente o PRD, a especificação funcional, a arquitetura, o modelo de dados, o plano de implementação e os critérios de aceite.

Existe um projeto anterior do usuário que pode servir como referência conceitual para scanner, processamento de mídia, parsing de nomes, hashes e metadados:

`https://github.com/rubensmneto/MediaProcessor`

Não copie componentes automaticamente. Inspecione o projeto e reutilize apenas código ou padrões compatíveis com a licença e com a arquitetura deste novo projeto.

## Primeira tarefa

Antes de implementar funcionalidades, faça uma inspeção do repositório atual e produza um relatório curto contendo:

- estado atual do repositório;
- arquivos existentes;
- SDKs instalados;
- versão do .NET disponível;
- disponibilidade do Git e GitHub CLI;
- possibilidade de acessar o repositório MediaProcessor;
- riscos técnicos, especialmente ADB, arquivos OneDrive somente online e metadados Android.

Se o repositório estiver vazio, crie a solução inicial conforme a arquitetura documentada. Não faça alterações destrutivas.

## Regras de implementação

- Usar WPF com MVVM.
- Separar domínio, aplicação, infraestrutura e interface.
- Encapsular ADB atrás de interfaces testáveis.
- Encapsular a detecção/materialização do OneDrive somente online.
- Usar SQLite para índice e histórico.
- Implementar primeiro domínio e testes, depois integrações.
- Não armazenar fotos no banco.
- Não copiar, adicionar ou alterar fotos reais no repositório.
- Não versionar banco SQLite local, logs, backups, arquivos de usuário, segredos ou `bin/`/`obj/`.
- Ativar nullable reference types e tratar warnings novos.
- Usar cancelamento e progresso em operações longas.
- Validar todos os caminhos locais e remotos.
- Não executar comandos remotos de exclusão no Android.
- Registrar limitações de metadados sem fingir que o Android suporta data de criação em todos os casos.

## Ordem de trabalho

1. Confirmar requisitos e criar a solução mínima.
2. Implementar parser `yyyyMMdd_HHmmss` e filtro inclusivo de datas.
3. Implementar scanner recursivo e índice SQLite.
4. Implementar fake ADB, contrato ADB e detecção de dispositivos.
5. Implementar inventário de destino e planejamento.
6. Implementar prévia e decisões.
7. Implementar backup, transferência, validação por tamanho e histórico.
8. Implementar metadados e avisos.
9. Finalizar telas, cancelamento, progresso e histórico.
10. Testar, documentar e empacotar.

## Git e GitHub

O repositório remoto já foi criado pelo usuário. Antes de publicar, confirme o remote com:

```bash
git remote -v
```

Se não houver repositório local:

```bash
git init
git remote add origin https://github.com/rubensmneto/OneDriveAndroidSync.git
```

Não force push. Não reescreva histórico. Use commits pequenos e semanticamente claros, por exemplo:

- `chore: initialize WPF solution`
- `feat: add filename date parser`
- `feat: add incremental source index`
- `feat: add ADB device discovery`
- `feat: add synchronization preview`
- `feat: add transfer execution`
- `test: cover conflict planning`
- `docs: update implementation status`

Antes de cada commit relevante:

```bash
dotnet restore
dotnet build
dotnet test
```

Faça push somente depois de confirmar que o build e os testes estão aprovados. Nunca inclua credenciais ou tokens em arquivos, commits ou mensagens.

## Primeira entrega esperada

A primeira entrega deve conter uma solução compilável, projetos separados, parser de data com testes, modelos de domínio iniciais, `.gitignore`, README, configuração básica de logging e um esqueleto de persistência SQLite. Não implemente ADB real antes que o contrato e o fake estejam testados.

Ao terminar cada etapa, atualize `docs/STATUS.md` com o que foi feito, o que falta e riscos conhecidos.
