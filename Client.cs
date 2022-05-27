// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

namespace NetworkUDP {
    public partial class Eventos: EventArgs{
        public delegate void OnReceivedNewDataServer(string _data);
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

    public class ClientUDP {
        static UdpClient Client = new UdpClient();
        static IPEndPoint MyHost;
        static bool EncryptConnection = false;
        static string PrivateKeyXML, PublicKeyXML, PublicKeyXMLServer = "";
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
        public static bool ConnectServer(string IP, int PORT){
            try{
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
        public static bool SendData(string _text){
            try{
                byte[] buffer = TextToByte(_text, EncryptConnection, PublicKeyXMLServer);
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
                    string _text = ByteToText(data);
                    if(_text.StartsWith("<RSAKeyValue>") && _text.EndsWith("</RSAKeyValue>")){
                        PublicKeyXMLServer = _text;
                        EncryptConnection = true;
                        SendEncryption();
                        ChangeStatus(StatusConnection.Connected);
                    }else{
                    if(PublicKeyXMLServer == "")
                        OnReceivedNewDataServer?.Invoke(_text);
                    else
                        OnReceivedNewDataServer?.Invoke(ByteToText(data, true));
                    }
                }
                if(!EncryptConnection)
                    ChangeStatus(StatusConnection.Connected);
            }catch{
                try{
                    Client.BeginReceive(new AsyncCallback(ClientReceiveUDPCallback), null);
                    PublicKeyXMLServer = "";
                    EncryptConnection = false;
                    ChangeStatus(StatusConnection.Reconnecting);
                }catch{
                    ChangeStatus(StatusConnection.Disconnected);
                }
            }
            }
        static bool SendEncryption(){
            try{
                byte[] buffer = TextToByte(PublicKeyXML);
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
      static string ByteToText(byte[] _byte, bool _decrypt = false){
         if(_decrypt){
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(PrivateKeyXML);
            return Encoding.UTF8.GetString(RSA.Decrypt(_byte, true));
         }else{
            return Encoding.UTF8.GetString(_byte);
         }
      }
      static byte[] TextToByte(string _text, bool _encrypt = false, string _keyClient = ""){
         if(_encrypt){
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_keyClient);
            return rsa.Encrypt(Encoding.UTF8.GetBytes(_text), true);
         }else{
            return Encoding.UTF8.GetBytes(_text);
         }
      }
    }
}