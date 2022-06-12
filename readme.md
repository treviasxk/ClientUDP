# ClientUDP
ClientUDP é um client criado em .NET Core com C#, é um projeto simples e de fácil entendimento, os dados são enviado e recebido por string entre o servidor e o client, para organizar os dados recomendo que serialize as class ou object para JSON ou XML, a conexão possui criptografia RSA de ambas conexão e é perfeito para criação de servidores de jogos como por exemplo a Unity 3D.

 ![Preview](screenshots/ServerUDP.jpg)
 ![Preview](screenshots/ClientUDP.jpg)

### Como testar
Baixe os dois projetos ([ServerUDP](https://github.com/treviasxk/ServerUDP) e [ClientUDP](https://github.com/treviasxk/ClientUDP)) e apenas compile no Visual Studio Code ou Visual Studio IDE que já funcionará como demonstração, entre a comunicação do Server para o Client.
### Utilizar na Unity
Baixe a DLL em [Releases](https://github.com/treviasxk/ClientUDP/releases), coloque a DLL dentro da pasta Assets de seu projeto Unity, assim `Assets/bin/debug/ClientUDP.dll`, Depois é só importar a biblioteca `using NetworkUDP;` nos seus scripts.
### Utilizar como referencia
Baixe o projeto que você quer referenciar ([ServerUDP](https://github.com/treviasxk/ServerUDP) ou [ClientUDP](https://github.com/treviasxk/ClientUDP)). E dentro do seu projeto utilize o comando `dotnet add reference <local>/ClientUDP`.

## Documentação

| Ações | Descrição |
|-----------|---------------|
| ConnectServer(String ip, Int port) | Conecta no servidor com um IP e Porta especifico.|
| DisconnectServer() | Desconecta o client do servidor.|
| SendText(String text) | Envia uma string para o servidor.|
| MainThread() | Utilizado para definir a thread principal que irá executar as ações do RunOnMainThread(). Coloque essa ação dentro da função void Update() na Unity.|
| RunOnMainThread(Action action) | Executa ações dentro da thread principal do software, é utilizado para manipular objetos 3D na Unity.|

| Variáveis | Descrição|
|------|-----|
| Status(StatusConnection status) | Estado atual do servidor, Connected, Disconnected ou Reconnecting.|

| Eventos | Descrição|
|------|-----|
| OnStatusConnection(StatusConnection status) | O evento é chamado quando o status do servidor muda: Connected, Disconnected ou Reconnecting e também será retornado um StatusConnection no parâmetro da função.|
| OnReceivedNewDataServer(String text) | O evento é chamado quando uma string é recebido do servidor e também será retornado uma string no parâmetro da função.|