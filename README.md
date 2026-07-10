# Quote Monitor

Aplicação de console desenvolvida em C# para monitorar cotações de ações da B3 e enviar notificações por e-mail quando um preço de compra ou venda for atingido.

### Executando pelo código-fonte

Uma versão compilada da aplicação está disponível na pasta:

```text
Publish/
```

Nessa pasta encontram-se o executável `QuoteMonitor.exe` e todos os arquivos de configuração necessários para sua execução.

Para monitorar um único ativo:

```bash
.\QuoteMonitor.exe PETR4 39.45 39.43
```

Onde:

- `PETR4`: símbolo do ativo.
- `39.45`: preço para venda.
- `39.43`: preço para compra.

Para monitorar todos os ativos definidos em `quotes-base.json`:

```bash
.\QuoteMonitor.exe all
```

Caso queira adicionar outro e-mail para recebimento o aviso é em `emails-base.json`.

### 1. Consulta das cotações

O primeiro passo foi implementar a comunicação com uma API de cotações.

Foi criada a interface `IQuoteProvider` para desacoplar o restante da aplicação do provedor utilizado. Dessa forma, caso seja necessário trocar a API futuramente, basta criar uma nova implementação da interface, sem alterar a lógica de monitoramento.

A API gratuita escolhida possui suporte apenas para alguns ativos (PETR4, VALE3 e MGLU3), mas essa limitação fica isolada dentro da implementação do provider.

Além disso, falhas de comunicação com a API não interrompem a execução do monitor. Caso uma consulta falhe, a aplicação avisa e continua executando normalmente e tenta novamente na próxima iteração.

A parte de lidar com falhas no projeto existe, mas é simples e objetiva.

---

### 2. TrackingQuote

A classe `TrackingQuote` representa um ativo monitorado.

Ela concentra todas as informações necessárias para aquele ativo, como:

- símbolo;
- preço de compra;
- preço de venda;
- último preço monitorado.

Também é responsável pelas regras de negócio relacionadas ao ativo, decidindo:

- quando existe oportunidade de compra;
- quando existe oportunidade de venda;
- quando um preço realmente mudou, evitando notificações repetidas.

Dessa forma, a lógica de negócio permanece centralizada no domínio, enquanto o serviço de monitoramento apenas coordena o fluxo da aplicação.

---

### 3. Envio de e-mails

Após concluir o monitoramento das cotações, foi implementado o envio de notificações por e-mail.

A responsabilidade foi dividida em partes:

- `EmailSender`: responsável apenas pelo envio.
- `EmailMessage`: responsável pela criação das mensagens.
- arquivos de configuração: responsáveis pelas credenciais e destinatários.

Adicionar novos tipos de mensagens ou alterar o provedor de e-mail sem modificar o restante da aplicação pelos arquivos de configuração.

Falhas durante o envio também não interrompem o monitoramento. Caso o envio não seja realizado, o erro é registrado e a aplicação continua funcionando normalmente.

Apenas erros de não ter informações para conexão é interrompido.

---

### 4. Configuração

As configurações foram separadas conforme sua responsabilidade.

- `.env`
    - credenciais do remetente;
- `appsettings.json`
    - configurações SMTP;
- `emails-base.json`
    - lista de destinatários;
- `quotes-base.json`
    - ativos monitorados.

Para este projeto de console, o `.env` foi utilizado para evitar deixar credenciais diretamente no código.

Essa não seria a abordagem ideal em um ambiente de produção. O mais adequado seria utilizar variáveis de ambiente configuradas no servidor ou um serviço de gerenciamento de segredos, como Azure Key Vault.

Porém, como este é um projeto de console disponibilizado em um repositório para avaliação, não existe um ambiente de deploy responsável por fornecer essas credenciais automaticamente. Sem alguma configuração local, a funcionalidade de envio de e-mails não poderia ser testada por outra pessoa.

Por isso, a decisão foi utilizar um `.env` local no repositório, mantendo fora do `.gitignore`. Dessa forma, o projeto continua funcional.

---

### 5. Serviço principal

O `QuoteMonitorService` é o responsável por coordenar todo o ciclo da aplicação.

Seu funcionamento consiste em:

1. consultar a cotação;
2. verificar se houve alteração de preço;
3. aplicar as regras de compra e venda;
4. enviar notificações quando necessário;

---

### 6. Monitoramento de múltiplos ativos

Inicialmente o projeto monitorava apenas um ativo informado pela linha de comando.

Posteriormente foi adicionada a opção:

```bash
QuoteMonitor.exe all
```

Nesse modo, o sistema lê automaticamente o arquivo `quotes-base.json` e cria um monitor para cada ativo configurado, permitindo acompanhar vários ativos simultaneamente sem alterar o código.
