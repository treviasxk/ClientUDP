// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace NetworkUDP {
    public partial class Eventos: EventArgs{
        public delegate void OnReceivedNewDataServer(object _data);
        public delegate void OnStatusConnection(StatusConnection _status);
    }

    public enum StatusConnection{
        Disconnected = 0,
        Connected = 1,
        Reconnecting = 2,
    }

    public class ClientUDP {
        static object datagram = new object();
        /// <summary>
        /// O evento é chamado quando um datagrama é recebido do servidor e também será retornado um datagram como object no parâmetro da função.
        /// </summary>
        public static event Eventos.OnReceivedNewDataServer OnReceivedNewDataServer;
        /// <summary>
        /// O evento é chamado quando o status do servidor muda: Connected, Disconnected ou Reconnecting e também será retornado um StatusConnection no parâmetro da função.
        /// </summary>
        public static event Eventos.OnStatusConnection OnStatusConnection;
        static UdpClient Client = new UdpClient();
        static IPEndPoint MyHost;
        /// <summary>
        /// Estado atual do servidor, Connected, Disconnected ou Reconnecting.
        /// </summary>
        public static StatusConnection Status = StatusConnection.Disconnected;
        /// <summary>
        /// Conecta no servidor com um IP e Porta especifico e insire a class do datagram.
        /// </summary>
        public static bool ConnectServer(string IP, int PORT, object _data){
            try{
                datagram = _data;
                IPEndPoint Host = new IPEndPoint(IPAddress.Parse(IP), PORT);
                MyHost = Host;
                Client.Connect(Host);
                Client.BeginReceive(new AsyncCallback(ClientReceiveUDPCallback), null);
                Thread t = new Thread(new ThreadStart(SendOnline));
                t.Start();
                return true;
            }catch{
                ChangeStatus(StatusConnection.Disconnected);
                return false;
            }
        }
        /// <summary>
        /// Deconectar o Client do servidor.
        /// </summary>
        public static void DisconnectServer(){
            Client.Close();
        }
        /// <summary>
        /// Envie o datagram para um Client especifico. 
        /// </summary>
        public static bool SendData(object _data){
            try{
                byte[] buffer = DatagramJsonToByte(_data);
                Client.Send(buffer, buffer.Length);
                return true;
            }catch{
                return false;
            }
        }
        private static void ClientReceiveUDPCallback(IAsyncResult _result){
            try{
                byte[] data = Client.EndReceive(_result, ref MyHost);
                Client.BeginReceive(new AsyncCallback(ClientReceiveUDPCallback), null);
                ChangeStatus(StatusConnection.Connected);
                if(data.Length > 0){
                    object _data = ByteJsonToDatagram(data);
                    OnReceivedNewDataServer?.Invoke(_data);
                }
            }catch{
                try{
                    Client.BeginReceive(new AsyncCallback(ClientReceiveUDPCallback), null);
                    ChangeStatus(StatusConnection.Reconnecting);
                }catch{
                    ChangeStatus(StatusConnection.Disconnected);
                }
            }
        }
        static void SendOnline(){
            while(true){
                try{
                    byte[] buffer = new byte[] {};
                    Client.Send(buffer, buffer.Length);
                }catch{}
                Thread.Sleep(1000);
            }
        }
        static void ChangeStatus(StatusConnection _status){
            if(Status != _status){
                Status = _status;
                OnStatusConnection?.Invoke(_status);
            }
        }
        static object ByteJsonToDatagram(byte[] _json){
            try{
                var utf8Reader = new Utf8JsonReader(_json);
                return JsonSerializer.Deserialize(ref utf8Reader, datagram.GetType());
            }catch{
                return null;
            }
        }
        static byte[] DatagramJsonToByte(object _data){
            try{
                return JsonSerializer.SerializeToUtf8Bytes(_data);
            }catch{
                return null;
            }
        }
    } 
}