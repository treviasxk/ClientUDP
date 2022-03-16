using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Text;
using System.Xml;

namespace NetworkUDP{
    public partial class Eventos: EventArgs{
        public delegate void OnReceivedNewDataServer(object _data);
    }

    public class ClientUDP {
        static object datagram = new object();
        public static event Eventos.OnReceivedNewDataServer? OnReceivedNewDataServer;
        static UdpClient Client = new UdpClient();
        static IPEndPoint? MyHost;

        public static void ConnectServer(string IP, int PORT, object _data){
            try{
                datagram = _data;
                IPEndPoint Host = new IPEndPoint(IPAddress.Parse(IP), PORT);
                MyHost = Host;
                Client.Connect(Host);
                Client.BeginReceive(new AsyncCallback(ClientReceiveUDPCallback), null);
            }catch{
            }
        }
        private static void ClientReceiveUDPCallback(IAsyncResult _result){
            try{
                object _data = XmlToDatagram(Encoding.UTF8.GetString(Client.EndReceive(_result, ref MyHost)));
                if(_data != null){
                    OnReceivedNewDataServer?.Invoke(_data);
                }
                Client.BeginReceive(new AsyncCallback(ClientReceiveUDPCallback), null);
            }catch{
                //No connection
            }
        }
        public static void SendData(object _data){
            byte[] buffer = Encoding.UTF8.GetBytes(DatagramToXml(_data));
            Client.Send(buffer, buffer.Length);
        }

        static object XmlToDatagram(string _xml){
            XmlSerializer x = new XmlSerializer(datagram.GetType());
            object _data = x.Deserialize(new StringReader(_xml));
            if(_data != null){
                return _data;
            }else{
                return new object();
            }
        }

        static string DatagramToXml(object _data){
            XmlSerializer x = new XmlSerializer(datagram.GetType());
            StringWriter _xml = new StringWriter();
            var textWriter = XmlWriter.Create(_xml);
            x.Serialize(textWriter, _data);
            return _xml.ToString();
        }
    } 
}