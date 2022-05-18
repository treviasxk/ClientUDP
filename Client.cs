// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Security.Cryptography;

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
   public class Server{
      public IPEndPoint IP;
      public float Ping;
      public long Time;
      public string PublicKeyXML = "";
   }

   class Connection{
        public bool EncryptConnection { get; set; }
        public string PublicKeyXML { get; set; }
        public byte[] Datagram { get; set; }
   }

    public class ClientUDP {
        static UdpClient Client = new UdpClient();
        static IPEndPoint MyHost;
        static bool EncryptConnection;
        static string PrivateKeyXML, PublicKeyXML, PublicKeyXMLServer = "";
        static Type DataType;
        /// <summary>
        /// O evento é chamado quando um Datagrama é recebido do servidor e também será retornado um Datagram como object no parâmetro da função.
        /// </summary>
        public static event Eventos.OnReceivedNewDataServer OnReceivedNewDataServer;
        /// <summary>
        /// O evento é chamado quando o status do servidor muda: Connected, Disconnected ou Reconnecting e também será retornado um StatusConnection no parâmetro da função.
        /// </summary>
        public static event Eventos.OnStatusConnection OnStatusConnection;
        /// <summary>
        /// Estado atual do servidor, Connected, Disconnected ou Reconnecting.
        /// </summary>
        public static StatusConnection Status = StatusConnection.Disconnected;
        /// <summary>
        /// Conecta no servidor com um IP e Porta especifico e insire a class do Datagram.
        /// </summary>
        public static bool ConnectServer(string IP, int PORT, object _type, bool _encrypt = true){
            try{
                EncryptConnection = _encrypt;
                DataType = _type.GetType();
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                PrivateKeyXML = RSA.ToXmlString(true);
                PublicKeyXML = RSA.ToXmlString(false);
                MyHost = new IPEndPoint(IPAddress.Parse(IP), PORT);
                Client.Connect(MyHost);
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
        /// Envie o Datagram para um Client especifico. 
        /// </summary>
        public static bool SendData(object _data){
            try{
                Connection _connection = new Connection();
                _connection.EncryptConnection = EncryptConnection;
                byte[] buffer = DatagramJsonToByte(_connection, _data, EncryptConnection);
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
                if(data.Length > 0){
                    Connection _connection = (Connection)ByteJsonToDatagram(data, new Connection().GetType());
                    if(_connection.PublicKeyXML == null){
                        object _data = ByteJsonToDatagram(_connection.Datagram, DataType, _connection.EncryptConnection);
                        OnReceivedNewDataServer?.Invoke(_data);
                    }else{
                        PublicKeyXMLServer = _connection.PublicKeyXML;
                        SendEncryption();
                        ChangeStatus(StatusConnection.Connected);
                    }
                    Console.WriteLine(_connection.EncryptConnection);
                }
                if(!EncryptConnection)
                    ChangeStatus(StatusConnection.Connected);
            }catch{
                try{
                    Client.BeginReceive(new AsyncCallback(ClientReceiveUDPCallback), null);
                    ChangeStatus(StatusConnection.Reconnecting);
                }catch{
                    ChangeStatus(StatusConnection.Disconnected);
                }
            }
        }
        static bool SendEncryption(){
            try{
                Connection _connection = new Connection();
                object _data = new object();
                _connection.PublicKeyXML = PublicKeyXML;
                _connection.EncryptConnection = true;
                byte[] buffer = DatagramJsonToByte(_connection, _data);
                Client.Send(buffer, buffer.Length);
                return true;
            }catch{
                return false;
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
        static object ByteJsonToDatagram(byte[] _json, Type _type, bool _decrypt = false){
            if(_decrypt){
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(PrivateKeyXML);
                var utf8Reader = new Utf8JsonReader(RSA.Decrypt(_json, true));
                return JsonSerializer.Deserialize(ref utf8Reader, _type);
            }else{
                var utf8Reader = new Utf8JsonReader(_json);
                return JsonSerializer.Deserialize(ref utf8Reader, _type);
            }
        }
        static byte[] DatagramJsonToByte(Connection _config, object _data, bool _encrypt = false){
            if(_encrypt){
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(PublicKeyXMLServer);
                _config.Datagram = rsa.Encrypt(JsonSerializer.SerializeToUtf8Bytes(_data), true);
                return JsonSerializer.SerializeToUtf8Bytes(_config);
            }else{
                _config.Datagram = JsonSerializer.SerializeToUtf8Bytes(_data);
                return JsonSerializer.SerializeToUtf8Bytes(_config);   
            }
        }
    }
}