using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Text;
using System.Xml;

public partial class Eventos: EventArgs{
   public delegate void OnReceivedNewDataServer(datagram _data);
}

public class ClientUDP {
    public static event Eventos.OnReceivedNewDataServer? OnReceivedNewDataServer;
    static UdpClient Client = new UdpClient();
    static IPEndPoint? MyHost;
    public static bool ConnectServer(string IP, int PORT){
        try{
            IPEndPoint Host = new IPEndPoint(IPAddress.Parse(IP), PORT);
            MyHost = Host;
            Client.Connect(Host);
            Client.BeginReceive(new AsyncCallback(ClientReceiveUDPCallback), null);
            return true;
        }catch{
            return false;
        }
    }
    private static void ClientReceiveUDPCallback(IAsyncResult _result){
        try{
            datagram _data = XmlToDatagram(Encoding.UTF8.GetString(Client.EndReceive(_result, ref MyHost)));
            if(_data != null){
                OnReceivedNewDataServer?.Invoke(_data);
            }
            Client.BeginReceive(new AsyncCallback(ClientReceiveUDPCallback), null);
        }catch{
            //No connection
        }
    }
    public static void SendData(datagram _data){
        byte[] buffer = Encoding.UTF8.GetBytes(DatagramToXml(_data));
        Client.Send(buffer, buffer.Length);
    }

    static datagram XmlToDatagram(string _xml){
        XmlSerializer x = new XmlSerializer(typeof(datagram));
        var _data = (datagram?)x.Deserialize(new StringReader(_xml));
        if(_data != null){
            return _data;
        }else{
            return new datagram();
        }
    }

    static string DatagramToXml(datagram _data){
        XmlSerializer x = new XmlSerializer(typeof(datagram));
        StringWriter _xml = new StringWriter();
        var textWriter = XmlWriter.Create(_xml);
        x.Serialize(textWriter, _data);
        return _xml.ToString();
    }
}