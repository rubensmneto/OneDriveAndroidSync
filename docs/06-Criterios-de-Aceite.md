# OneDriveAndroidSync — Critérios de aceite

## Origem e filtro

- Dado que uma pasta raiz válida foi configurada, quando a análise for iniciada, então todas as subpastas serão pesquisadas.
- Dado um intervalo de datas, arquivos cujo nome contenha data dentro do intervalo inclusivo deverão ser apresentados.
- Arquivos no limite inicial e final deverão ser incluídos.
- Arquivos fora do padrão deverão aparecer separados e permitir decisão em lote.
- Datas inválidas deverão aparecer como revisão, sem processamento automático.
- Arquivos somente online deverão ser materializados antes da transferência ou aparecer como erro explicável.

## Incrementalidade

- Arquivo com mesmo caminho, tamanho e data de modificação já concluído não deverá ser transferido novamente.
- Arquivo alterado deverá ser reavaliado.
- Hash deverá ser usado em conflitos ou dúvidas.
- Primeira execução deverá construir o índice sem exigir histórico prévio.

## Android e ADB

- Dispositivos ADB autorizados deverão ser listados.
- Dispositivo não autorizado deverá gerar instrução clara.
- O usuário deverá escolher o dispositivo antes da execução.
- A pasta de destino deverá ser listada e selecionável.
- Desconexão deverá interromper com segurança e salvar o progresso.
- A operação não deverá continuar em outro dispositivo automaticamente.

## Conflitos e segurança

- Nenhum arquivo remoto deverá ser excluído automaticamente.
- Conflito de conteúdo deverá ser visível antes da cópia.
- Conflito de metadados deverá ser visível antes da cópia.
- Substituição autorizada deverá criar backup local antes da alteração.
- Colisões de nomes diferentes deverão preservar ambos usando subpastas derivadas da origem.

## Transferência e metadados

- O conteúdo deverá ser copiado sem recompressão.
- A validação mínima deverá comparar tamanho local e remoto.
- O sistema deverá tentar preservar metadados verificáveis.
- Limitações do Android deverão ser registradas sem esconder o sucesso da cópia.
- Erro em um arquivo não deverá impedir os demais.
- Cancelamento deverá evitar iniciar itens novos e preservar o estado.

## Interface e histórico

- A prévia deverá mostrar resumo e lista detalhada.
- A interface não deverá congelar durante análise ou transferência.
- O progresso deverá ser visível.
- O resumo final deverá mostrar copiados, iguais, substituídos, ignorados, falhos e avisos.
- A sessão deverá ser consultável no SQLite.
- Configurações deverão persistir entre execuções.
