# OneDriveAndroidSync — Especificação funcional

## 1. Fluxo principal

O usuário abre a aplicação, configura a pasta raiz do OneDrive, o caminho do `adb.exe`, a pasta de backup e o destino Android. Em seguida seleciona um intervalo obrigatório de datas e inicia a análise.

A aplicação varre a origem, solicita ao OneDrive o download de arquivos somente online, extrai datas dos nomes, consulta o índice SQLite e classifica cada item. Depois exibe a prévia. O usuário resolve arquivos fora do padrão em lote e conflitos individualmente. Após a confirmação, o sistema transfere os itens aprovados e apresenta o resumo final.

## 2. Estados de arquivo

Cada arquivo poderá assumir estados como:

- `Pending`: pendente de processamento.
- `OnlineOnly`: conteúdo não disponível localmente.
- `InvalidName`: nome sem data válida.
- `AlreadySynchronized`: conteúdo e metadados equivalentes.
- `New`: ainda não identificado no destino.
- `ContentConflict`: conteúdo diferente no destino.
- `MetadataConflict`: conteúdo igual, metadados diferentes.
- `Approved`: aprovado pelo usuário.
- `Transferred`: copiado e validado.
- `MetadataWarning`: conteúdo transferido, mas algum metadado não pôde ser aplicado.
- `Failed`: falha registrada.
- `Cancelled`: não processado por cancelamento.

## 3. Prévia

A prévia deverá mostrar resumo por categoria e tabela detalhada. A tabela deverá incluir nome, caminho de origem, tamanho, data extraída, destino previsto, estado, motivo e ação.

Ações em lote: incluir ou ignorar arquivos sem padrão válido. Ações individuais: substituir, preservar ou tratar conflitos conforme decisão do usuário.

## 4. Transferência

A transferência será feita por comandos ADB. O aplicativo deverá criar diretórios de destino quando necessário, copiar o arquivo e validar o tamanho remoto.

A operação deverá usar nomes seguros e impedir que caminhos de origem criem traversal no destino. O destino final será normalizado antes de qualquer comando.

Quando houver substituição, o arquivo remoto antigo deverá ser baixado para uma pasta de backup organizada por data e identificador da sessão.

## 5. Metadados

O arquivo deverá ser copiado sem reprocessamento. O sistema deverá tentar preservar EXIF, `DateTimeOriginal`, GPS, fabricante, modelo e datas físicas, quando houver suporte.

Após a cópia, a validação mínima será o tamanho. Falhas na aplicação de metadados deverão gerar alerta, sem invalidar automaticamente a cópia do conteúdo.

## 6. ADB e dispositivo

A aplicação deverá listar dispositivos retornados por `adb devices -l`, distinguindo autorizado, não autorizado e indisponível. O usuário deverá selecionar o dispositivo.

Antes da execução, validar conexão. Durante a execução, monitorar o dispositivo. Se ele desaparecer, interromper com segurança, persistir o progresso e mostrar instrução para reconectar e iniciar nova execução.

## 7. OneDrive somente online

A aplicação deverá detectar arquivos que não possuem conteúdo local. A implementação inicial poderá usar atributos e tentativa controlada de leitura para confirmar disponibilidade. O pedido de download deverá ser executado por mecanismo compatível com o cliente OneDrive instalado. O desenho técnico deverá encapsular essa integração para permitir evolução futura.

## 8. Cancelamento e erros

O cancelamento deverá evitar iniciar novos arquivos e concluir o item atual quando possível. Cada falha deverá ser persistida com mensagem técnica e mensagem amigável. O processamento deverá continuar com os demais arquivos, exceto em perda do dispositivo, cancelamento ou erro estrutural.

## 9. Configurações

Salvar origem, ADB, destino, backup, timeouts, regras de conflito, validação, logs e demais opções. As configurações não deverão conter tokens ou credenciais.

## 10. Telas sugeridas

- Dashboard/visão geral.
- Configurações.
- Seleção de intervalo e origem.
- Seleção do dispositivo e pasta Android.
- Prévia da sincronização.
- Progresso da execução.
- Resumo final e histórico.
