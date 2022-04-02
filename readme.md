# ClientUDP
ClientUDP é um client criado em .NET Core com C#, é um projeto simples e de fácil entendimento, os dados são armazenado em uma Class chamado datagram que contém as variáveis que serão enviado para o servidor, essa class é convertido para Json e do Json é convertido para bytes[], é perfeito para criação de servidores de jogos como por exemplo a Unity 3D.

 ![Preview](screenshots/ServerUDP.jpg)
 ![Preview](screenshots/ClientUDP.jpg)

### Como utilizar
Baixe os dois projetos ([ServerUDP](https://github.com/treviasxk/ServerUDP) e [ClientUDP](https://github.com/treviasxk/ClientUDP)) e apenas compile que já funcionará como demonstração, entre a comunicação do Server para o Client.

## ClientUDP

| Ações | Descrição |
|-----------|---------------|
| ConnectServer(String, Int, Class) | Conecta no servidor com um IP e Porta especifico e insire a class do datagram.|
| DisconnectServer() | Deconectar o client do servidor.|
| SendData(object) | Envie o datagram para um Client especifico.|

| Variáveis | Descrição|
|------|-----|
| datagram(Class) | É uma class que contém variáveis que você deseja enviar. O ServerUDP e ClientUDP tem que ter os mesmo valores da class para o envio funcionar perfeitamente.|
| Status(StatusConnection) | Estado atual do servidor, Connected, Disconnected ou Reconnecting.|

| Eventos | Descrição|
|------|-----|
| OnStatusConnection(object) | O evento é chamado quando o status do servidor muda: Connected, Disconnected ou Reconnecting e também será retornado um StatusConnection no parâmetro da função.|
| OnReceivedNewDataServer(object) | O evento é chamado quando um datagrama é recebido do servidor e também será retornado um datagram como object no parâmetro da função.|

## ServerUDP

| Ações | Descrição |
|-----------|---------------|
| StartServer(String, Int, Class) | Inicie o servidor com um IP e Porta especifico e insire a class do datagram.|
| SendData(object, IPEndPoint) | Envie o datagram para um Client especifico. |
| SendDataAll(object) | Envie o datagram para todos os Clients conectado no servidor.|

| Variáveis | Descrição|
|------|-----|
| Classes | dtg é uma class que contém variáveis que você deseja enviar. O ServerUDP e ClientUDP tem que ter os mesmo valores da class para o envio funcionar perfeitamente.|
| ListClients (Dictionary<IPEndPoint, long>) | Lista de todos os Clients que estão conectado no servidor.|

| Eventos | Descrição|
|------|-----|
| OnDisconnectedClient(IPEndPoint) | O evento é chamado quando um Client se desconecta do servidor e também é retornado o endereço do Client IPEndPoint.|
| OnConnectedClient(IPEndPoint) | O evento é chamado quando um Client se conecta no servidor e também é retornado o endereço do Client IPEndPoint.|
| OnReceivedNewDataClient(object, IPEndPoint) | O evento é chamado quando um datagrama é recebido de um Client e também será retornado um datagram como object e o endereço do Client no parâmetro da função.|