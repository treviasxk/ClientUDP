// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using static Eventos;

public class datagram {
    public string? name { get; set; }
    public string? msg { get; set; }
}

class Program {
   static void Main(string[] args){
        if(ClientUDP.ConnectServer("127.0.0.1", 26950)){
            ClientUDP.OnReceivedNewDataServer += new OnReceivedNewDataServer(OnReceivedNewDataServer);
            var _data = new datagram();
            _data.name = "VOCÊ";
            _data.msg = "OI";
            Console.WriteLine("{0}: {1}", _data.name, _data.msg);
            ClientUDP.SendData(_data);
            Console.ReadKey();
        }
   }

   //========================= Evento =========================
   static void OnReceivedNewDataServer(datagram _data){
      Console.WriteLine("{0}: {1}", _data.name, _data.msg);
   }
}