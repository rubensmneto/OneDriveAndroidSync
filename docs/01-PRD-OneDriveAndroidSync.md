# OneDriveAndroidSync — PRD

## 1. Visão geral

O OneDriveAndroidSync será uma aplicação desktop Windows em C#/.NET com interface WPF para transferir arquivos de uma pasta local sincronizada pelo OneDrive para um dispositivo Android conectado por USB usando ADB.

A aplicação terá sincronização unidirecional, do computador para o Android. Ela não apagará arquivos do celular e não verificará se o aplicativo de nuvem concluiu o backup. Sua responsabilidade termina depois que o arquivo for copiado e validado no dispositivo.

Repositório: https://github.com/rubensmneto/OneDriveAndroidSync

## 2. Problema

O usuário possui uma quantidade muito grande de fotos e outros arquivos no OneDrive. Parte desse conteúdo já foi transferida manualmente para o celular. É necessário processar apenas um intervalo de datas, identificar o que já existe no aparelho, evitar duplicatas, tratar conflitos e preservar o máximo possível dos metadados originais.

A maioria dos arquivos usa o padrão `yyyymmdd_hhmmss.ext`, que deve ser utilizado como fonte principal para o filtro de datas.

## 3. Objetivos

- Transferir arquivos da pasta local do OneDrive para o Android via ADB.
- Permitir processamento por intervalo obrigatório de datas, inclusivo.
- Fazer processamento incremental usando um índice SQLite local.
- Detectar arquivos já presentes no dispositivo antes de copiar.
- Tratar conflitos de conteúdo e de metadados antes da substituição.
- Fazer backup no computador antes de substituir arquivos do celular.
- Preservar conteúdo, EXIF e outros metadados sempre que tecnicamente possível.
- Mostrar uma prévia clara antes da execução.
- Continuar após erros individuais e manter histórico completo.

## 4. Não objetivos da primeira versão

- Acessar o OneDrive diretamente pela Microsoft Graph.
- Verificar se Google Fotos, OneDrive ou outro serviço concluiu o backup.
- Sincronização bidirecional.
- Exclusão automática de arquivos no Android.
- Monitoramento contínuo em segundo plano.
- Aplicativo próprio instalado no Android.
- Criptografia adicional do banco ou dos backups.

## 5. Usuário principal

Usuário técnico ou semitécnico que possui Windows, OneDrive instalado e arquivos disponíveis em uma pasta local. O usuário deverá configurar manualmente o Android Platform-Tools/ADB e autorizar a depuração USB no aparelho.

## 6. Requisitos de negócio

### Origem

A aplicação deverá aceitar uma única pasta raiz do OneDrive e pesquisar recursivamente todas as subpastas. Todos os arquivos serão considerados, independentemente da extensão.

Arquivos que aparecem como somente online deverão ser baixados automaticamente pelo OneDrive antes da transferência. O sistema deverá aguardar a disponibilidade local e registrar timeout ou erro quando necessário.

### Filtro de datas

Toda execução deverá exigir data inicial e data final. O intervalo será inclusivo e abrangerá os dias inteiros.

A data será extraída do nome no padrão `yyyymmdd_hhmmss.ext` e interpretada no horário local do Windows. Arquivos sem padrão válido ou com data inválida serão exibidos em categoria de revisão e poderão ser incluídos ou ignorados em lote.

### Processamento incremental

O índice local deverá considerar alterações usando caminho, tamanho e data de modificação. Hash será calculado seletivamente em dúvidas e conflitos.

Arquivos já concluídos ficarão fora por padrão. A execução processará somente pendentes ou arquivos alterados dentro do intervalo informado.

### Destino

O aplicativo deverá listar pastas acessíveis no armazenamento Android após o dispositivo ser selecionado. `DCIM/Camera` será a sugestão inicial, mas o usuário escolherá a pasta.

A preferência é uma pasta única. Em colisões de nome com conteúdos diferentes, deverão ser criadas subpastas derivadas do caminho de origem para impedir perda.

### Conflitos

Conflitos não serão substituídos automaticamente. O usuário deverá decidir individualmente na prévia. Quando a substituição for autorizada, o arquivo antigo do celular deverá ser baixado para backup no computador antes da alteração.

Se o conteúdo for igual, mas houver divergência de metadados, será considerado conflito para permitir atualização dos metadados.

### Segurança operacional

A sincronização será sempre do OneDrive para o Android. Arquivos que existem apenas no celular nunca serão removidos.

Se o dispositivo selecionado for desconectado, a operação deverá parar com segurança e preservar o histórico parcial. O aplicativo nunca deverá continuar em outro dispositivo sem nova seleção.

## 7. Indicadores de sucesso

- Nenhum arquivo confirmado como concluído é copiado novamente sem motivo.
- Nenhum arquivo do celular é excluído automaticamente.
- Todo conflito é visível antes da substituição.
- Todo arquivo substituído possui backup local anterior.
- Toda execução possui histórico no SQLite.
- Falhas individuais não impedem os demais arquivos de serem processados.
- O usuário consegue identificar arquivos copiados, ignorados, conflitantes, pendentes e falhos.
